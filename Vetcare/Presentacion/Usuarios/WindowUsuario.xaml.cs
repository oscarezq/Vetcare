using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Utilidades;

namespace Vetcare.Presentacion.Usuarios
{
    /// <summary>
    /// Ventana para crear o editar usuarios del sistema
    /// </summary>
    public partial class WindowUsuario : Window
    {
        // Objeto usuario que se está creando o editando
        private readonly Usuario _usuario;

        // Servicios de acceso a lógica de negocio
        private readonly UsuarioService _usuarioService = new();
        private readonly RolService _rolService = new();
        private readonly VeterinarioService _veteService = new();

        // Indica si estamos en modo edición o creación
        private readonly bool _esEdicion;

        // Valida email
        [System.Text.RegularExpressions.GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
        private static partial System.Text.RegularExpressions.Regex EmailRegex();

        // Valida teléfono (9 dígitos)
        [System.Text.RegularExpressions.GeneratedRegex(@"^[0-9]{9}$")]
        private static partial System.Text.RegularExpressions.Regex TelefonoRegex();

        // Valida colegiado (9 dígitos)
        [System.Text.RegularExpressions.GeneratedRegex(@"^[0-9]{9}$")]
        private static partial System.Text.RegularExpressions.Regex ColegiadoRegex();

        /// <summary>
        /// Constructor para crear un nuevo usuario
        /// </summary>
        public WindowUsuario()
        {
            InitializeComponent();

            // Inicializamos un usuario vacío
            _usuario = new();

            // Indicamos que es creación
            _esEdicion = false;

            // Cargamos los roles disponibles
            CargarRoles();

            // Cambiamos el título
            lblTitulo.Text = "NUEVO USUARIO";
        }

        /// <summary>
        /// Constructor para editar un usuario existente
        /// </summary>
        public WindowUsuario(Usuario usuarioExistente)
        {
            InitializeComponent();

            // Asignamos el usuario recibido
            _usuario = usuarioExistente;

            // Activamos modo edición
            _esEdicion = true;

            // Cargamos roles
            CargarRoles();

            // Deshabilitamos campos que no se deben modificar
            cbRol.IsEnabled = false;
            txtUsername.IsEnabled = false;

            // Ocultamos el panel de contraseña
            panelPassword.Visibility = Visibility.Collapsed;

            // Cargamos los datos del usuario en la UI
            CargarDatosEdicion();

            // Cambiamos el título
            lblTitulo.Text = "EDITAR USUARIO";
        }

        /// <summary>
        /// Carga los roles desde la base de datos
        /// </summary>
        private void CargarRoles()
        {
            try
            {
                var roles = _rolService.ListarRoles();

                // Si es creación, ocultamos el rol Administrador
                if (!_esEdicion)
                {
                    roles = roles
                        .Where(r => r.NombreRol != "Administrador")
                        .ToList();
                }

                // Asignamos los roles al ComboBox
                cbRol.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar roles: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Carga los datos del usuario en modo edición
        /// </summary>
        private void CargarDatosEdicion()
        {
            // Datos básicos
            txtUsername.Text = _usuario.Username;
            txtNombre.Text = _usuario.Nombre;
            txtApellidos.Text = _usuario.Apellidos;
            txtEmail.Text = _usuario.Email;
            txtTelefono.Text = _usuario.Telefono;
            cbRol.SelectedValue = _usuario.IdRol;

            // Si el usuario es veterinario
            if (EsRolVeterinario())
            {
                // Mostramos el panel profesional
                borderVeterinario.Visibility = Visibility.Visible;

                // Obtenemos datos profesionales
                var datosPro = _veteService.ObtenerPorIdUsuario(_usuario.IdUsuario);

                if (datosPro != null)
                {
                    txtEspecialidad.Text = datosPro.Especialidad;
                    txtColegiado.Text = datosPro.NumeroColegiado;
                }
                else
                {
                    // Limpiamos campos si no hay datos
                    txtEspecialidad.Text = string.Empty;
                    txtColegiado.Text = string.Empty;
                }
            }
            else
            {
                // Ocultamos panel si no es veterinario
                borderVeterinario.Visibility = Visibility.Collapsed;
                txtEspecialidad.Text = string.Empty;
                txtColegiado.Text = string.Empty;
            }
        }

        /// <summary>
        /// Evento al cambiar el rol seleccionado
        /// </summary>
        private void CbRol_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            // Mostramos u ocultamos la sección de veterinario
            borderVeterinario.Visibility = EsRolVeterinario() ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Evento al pulsar el botón Guardar
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validamos campos antes de continuar
            if (!ValidarCampos()) return;

            try
            {
                // Mapear datos del formulario al objeto usuario
                _usuario.Username = txtUsername.Text.Trim();
                _usuario.Nombre = txtNombre.Text.Trim();
                _usuario.Apellidos = txtApellidos.Text.Trim();
                _usuario.Email = txtEmail.Text.Trim();
                _usuario.Telefono = txtTelefono.Text.Trim();
                _usuario.IdRol = (int)cbRol.SelectedValue;

                // Gestión de contraseña
                if (!_esEdicion || !string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    string nuevoSalt = Seguridad.GenerarSalt();
                    _usuario.Salt = nuevoSalt;
                    _usuario.PasswordHash = Seguridad.Encriptar(txtPassword.Password, nuevoSalt);
                }

                bool operacionExitosa = false;

                if (!_esEdicion)
                {
                    // Insertar el usuario en bbdd
                    int nuevoId = _usuarioService.Insertar(_usuario);

                    if (nuevoId > 0)
                    {
                        operacionExitosa = true;

                        // Si es veterinario, guardamos datos profesionales
                        if (EsRolVeterinario())
                            GuardarDatosProfesionales(nuevoId);
                    }
                }
                else
                {
                    // Actualizar usuario en la bbdd
                    operacionExitosa = _usuarioService.Actualizar(_usuario);

                    if (operacionExitosa && EsRolVeterinario())
                        GuardarDatosProfesionales(_usuario.IdUsuario);
                }

                // Resultado final
                if (operacionExitosa)
                {
                    MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error crítico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Guarda o actualiza los datos profesionales del veterinario
        /// </summary>
        private void GuardarDatosProfesionales(int idUsuario)
        {
            Veterinario vete = new()
            {
                IdUsuario = idUsuario,
                Especialidad = txtEspecialidad.Text.Trim(),
                NumeroColegiado = txtColegiado.Text.Trim()
            };

            // Intentamos actualizar, si no existe lo insertamos
            if (!_veteService.Actualizar(vete))
                _veteService.Insertar(vete);
        }

        /// <summary>
        /// Comprueba si el rol seleccionado es Veterinario
        /// </summary>
        private bool EsRolVeterinario()
        {
            // En edición usamos el objeto cargado
            if (_esEdicion && _usuario != null)
            {
                if (!string.IsNullOrEmpty(_usuario.NombreRol) &&
                    _usuario.NombreRol.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // En creación usamos el ComboBox
            if (cbRol.SelectedItem is Rol rolSeleccionado)
                return rolSeleccionado.NombreRol!.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase);

            return false;
        }

        /// <summary>
        /// Valida todos los campos del formulario
        /// </summary>
        private bool ValidarCampos()
        {
            // Campos obligatorios
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
                return MostrarError("El username es obligatorio.");
            if (cbRol.SelectedValue == null)
                return MostrarError("Debe seleccionar un rol.");
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                return MostrarError("Los apellidos son obligatorios.");
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                return MostrarError("El email es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                return MostrarError("El teléfono es obligatorio.");

            // Validaciones de formato
            if (!EmailRegex().IsMatch(txtEmail.Text))
                return MostrarError("El formato del email no es válido (ejemplo@dominio.com).");

            if (!TelefonoRegex().IsMatch(txtTelefono.Text))
                return MostrarError("El teléfono debe tener exactamente 9 dígitos numéricos.");

            if (VerificarUsernameRepetido(txtUsername.Text))
                return MostrarError("El nombre de usuario ya está en uso por otra persona.");

            // Password obligatoria en creación
            if (!_esEdicion)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                    return MostrarError("Debe asignar una contraseña inicial.");
            }

            // Validaciones de veterinario
            if (EsRolVeterinario())
            {
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                    return MostrarError("La especialidad es obligatoria para veterinarios.");

                if (string.IsNullOrWhiteSpace(txtColegiado.Text))
                    return MostrarError("El número de colegiado es obligatorio.");

                if (!ColegiadoRegex().IsMatch(txtColegiado.Text))
                    return MostrarError("El número de colegiado debe tener 9 dígitos numéricos.");
            }

            return true;
        }

        /// <summary>
        /// Comprueba si el username ya existe
        /// </summary>
        private bool VerificarUsernameRepetido(string username)
        {
            if (_esEdicion && username == _usuario.Username)
                return false;

            return _usuarioService.ExisteUsername(username);
        }

        /// <summary>
        /// Muestra un mensaje de error
        /// </summary>
        private static bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        /// <summary>
        /// Cierra la ventana sin guardar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}