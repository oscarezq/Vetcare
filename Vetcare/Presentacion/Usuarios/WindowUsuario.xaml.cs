using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Utilidades; // Para usar tu clase Seguridad (Hash/Salt)

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
            CargarDatosEdicion();

            lblTitulo.Text = "EDITAR USUARIO";
            lblPassword.Text = "Cambiar contraseña";
            lblInfoPassword.Visibility = Visibility.Visible;
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
            txtUsername.Text = _usuario.Username;
            txtNombre.Text = _usuario.Nombre;
            txtApellidos.Text = _usuario.Apellidos;
            txtEmail.Text = _usuario.Email;
            txtTelefono.Text = _usuario.Telefono;
            cbRol.SelectedValue = _usuario.IdRol;

            // Si el usuario ya es veterinario, buscamos sus datos profesionales en su tabla específica
            if (EsRolVeterinario())
            {
                var datosPro = _veteService.ObtenerPorId(_usuario.IdUsuario);
                if (datosPro != null)
                {
                    txtEspecialidad.Text = datosPro.Especialidad;
                    txtColegiado.Text = datosPro.NumeroColegiado;
                }
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

            // Intentamos actualizar. Si el método devuelve 'false' (porque no existe), lo insertamos.
            if (!_veteService.Actualizar(vete))
            {
                _veteService.Insertar(vete);
            }
        }

        private bool EsRolVeterinario()
        {
            if (cbRol.SelectedItem is Rol rolSeleccionado)
            {
                // Comprobamos por nombre o por ID si lo conoces (ej: 2)
                return rolSeleccionado.NombreRol.Equals("Veterinario", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text)) return MostrarError("El username es obligatorio.");
            if (cbRol.SelectedValue == null) return MostrarError("Debe seleccionar un rol.");
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return MostrarError("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) return MostrarError("El email es obligatorio.");

            // Contraseña obligatoria solo en creación
            if (!_esEdicion && string.IsNullOrWhiteSpace(txtPassword.Password))
                return MostrarError("Debe asignar una contraseña inicial.");

            // Validaciones para veterinario
            if (EsRolVeterinario())
            {
                if (string.IsNullOrWhiteSpace(txtColegiado.Text))
                    return MostrarError("El número de colegiado es obligatorio para veterinarios.");
            }

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