using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Utilidades;

namespace Vetcare.Presentacion.Usuarios
{
    public partial class WindowUsuario : Window
    {
        private Usuario _usuario;
        private UsuarioService _usuarioService = new UsuarioService();
        private RolService _rolService = new RolService();
        private VeterinarioService _veteService = new VeterinarioService();

        private bool _esEdicion = false;

        public WindowUsuario()
        {
            InitializeComponent();
            _usuario = new Usuario();
            _esEdicion = false;
            CargarRoles();
            lblTitulo.Text = "NUEVO USUARIO";
        }

        public WindowUsuario(Usuario usuarioExistente)
        {
            InitializeComponent();
            _usuario = usuarioExistente;
            _esEdicion = true;

            CargarRoles();

            // IMPORTANTE: Primero configuramos la UI de edición
            cbRol.IsEnabled = false;
            txtUsername.IsEnabled = false;
            panelPassword.Visibility = Visibility.Collapsed;

            // Luego cargamos los datos
            CargarDatosEdicion();

            lblTitulo.Text = "EDITAR USUARIO";
        }

        private void CargarRoles()
        {
            try
            {
                cbRol.ItemsSource = _rolService.ListarRoles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar roles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CargarDatosEdicion()
        {
            // Rellenamos datos básicos
            txtUsername.Text = _usuario.Username;
            txtNombre.Text = _usuario.Nombre;
            txtApellidos.Text = _usuario.Apellidos;
            txtEmail.Text = _usuario.Email;
            txtTelefono.Text = _usuario.Telefono;
            cbRol.SelectedValue = _usuario.IdRol;

            // Lógica para Veterinario
            if (EsRolVeterinario())
            {
                // 1. Forzamos que el panel sea visible
                borderVeterinario.Visibility = Visibility.Visible;

                // 2. Intentamos traer los datos de la base de datos
                // Asegúrate de que _usuario.IdUsuario tenga el valor correcto de la DB
                var datosPro = _veteService.ObtenerPorIdUsuario(_usuario.IdUsuario);

                if (datosPro != null)
                {
                    txtEspecialidad.Text = datosPro.Especialidad;
                    txtColegiado.Text = datosPro.NumeroColegiado;
                }
                else
                {
                    // Si el servicio devuelve null, limpiamos por seguridad
                    txtEspecialidad.Text = string.Empty;
                    txtColegiado.Text = string.Empty;
                }
            }
            else
            {
                // Si no es veterinario, nos aseguramos de que esté oculto y vacío
                borderVeterinario.Visibility = Visibility.Collapsed;
                txtEspecialidad.Text = string.Empty;
                txtColegiado.Text = string.Empty;
            }
        }

        private void cbRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Mostramos u ocultamos los campos de veterinario según la selección
            borderVeterinario.Visibility = EsRolVeterinario() ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                // 1. Mapear datos del usuario
                _usuario.Username = txtUsername.Text.Trim();
                _usuario.Nombre = txtNombre.Text.Trim();
                _usuario.Apellidos = txtApellidos.Text.Trim();
                _usuario.Email = txtEmail.Text.Trim();
                _usuario.Telefono = txtTelefono.Text.Trim();
                _usuario.IdRol = (int)cbRol.SelectedValue;

                // Gestión de Seguridad
                if (!_esEdicion || !string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    string nuevoSalt = Seguridad.GenerarSalt();
                    _usuario.Salt = nuevoSalt;
                    _usuario.PasswordHash = Seguridad.Encriptar(txtPassword.Password, nuevoSalt);
                }

                bool operacionExitosa = false;

                if (!_esEdicion)
                {
                    // --- MODO CREACIÓN ---
                    int nuevoId = _usuarioService.Insertar(_usuario);
                    if (nuevoId > 0)
                    {
                        operacionExitosa = true;
                        // Si es veterinario, creamos su registro profesional vinculado al nuevo ID
                        if (EsRolVeterinario())
                        {
                            GuardarDatosProfesionales(nuevoId);
                        }
                    }
                }
                else
                {
                    // --- MODO EDICIÓN ---
                    operacionExitosa = _usuarioService.Actualizar(_usuario);
                    if (operacionExitosa && EsRolVeterinario())
                    {
                        // Actualizamos o creamos el perfil (por si antes no era vete)
                        GuardarDatosProfesionales(_usuario.IdUsuario);
                    }
                }

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

        private void GuardarDatosProfesionales(int idUsuario)
        {
            Veterinario vete = new Veterinario
            {
                IdUsuario = idUsuario,
                Especialidad = txtEspecialidad.Text.Trim(),
                NumeroColegiado = txtColegiado.Text.Trim()
            };

            if (!_veteService.Actualizar(vete))
            {
                _veteService.Insertar(vete);
            }
        }

        private bool EsRolVeterinario()
        {
            // Si estamos editando, lo más fiable es mirar el objeto que vino de la lista principal
            if (_esEdicion && _usuario != null)
            {
                // Comprobamos por el nombre del rol (asegúrate de que tu objeto Usuario tenga esta propiedad rellena)
                if (!string.IsNullOrEmpty(_usuario.NombreRol) &&
                    _usuario.NombreRol.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // Opcional: Si conoces el ID exacto del rol veterinario en tu DB (ejemplo: 2)
                // if (_usuario.IdRol == 2) return true;
            }

            // Si es nuevo usuario o el objeto no tiene el nombre, miramos el ComboBox
            if (cbRol.SelectedItem is Rol rolSeleccionado)
            {
                return rolSeleccionado.NombreRol.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool ValidarCampos()
        {
            // Validaciones de presencia
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

            // Validaciones de Formato y Lógica
            if (!EsEmailValido(txtEmail.Text))
                return MostrarError("El formato del email no es válido (ejemplo@dominio.com).");

            if (!EsTelefonoValido(txtTelefono.Text))
                return MostrarError("El teléfono debe tener exactamente 9 dígitos numéricos.");

            if (VerificarUsernameRepetido(txtUsername.Text))
                return MostrarError("El nombre de usuario ya está en uso por otra persona.");

            // 3. Validación de Password
            if (!_esEdicion)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                    return MostrarError("Debe asignar una contraseña inicial.");
            }

            // 4. Validaciones específicas de Veterinario
            if (EsRolVeterinario())
            {
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                    return MostrarError("La especialidad es obligatoria para veterinarios.");

                if (!EsColegiadoValido(txtColegiado.Text))
                    return MostrarError("El número de colegiado debe tener 9 dígitos numéricos.");
            }

            return true;
        }

        // Verifica si el username ya existe en la base de datos
        private bool VerificarUsernameRepetido(string username)
        {
            // Asumiendo que tienes un UsuarioService
            UsuarioService usuService = new UsuarioService();

            return usuService.ExisteUsername(username);
        }

        private bool EsEmailValido(string email)
        {
            // Regex estándar para email
            string expresion = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, expresion);
        }

        private bool EsTelefonoValido(string telefono)
        {
            // Verifica que sean exactamente 9 números
            return Regex.IsMatch(telefono, @"^[0-9]{9}$");
        }

        private bool EsColegiadoValido(string colegiado)
        {
            // Verifica que sean exactamente 9 números
            return Regex.IsMatch(colegiado, @"^[0-9]{9}$");
        }

        private bool EsPasswordSegura(string password)
        {
            // Al menos 8 caracteres, 1 Mayúscula, 1 Minúscula, 1 Número
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;

            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}