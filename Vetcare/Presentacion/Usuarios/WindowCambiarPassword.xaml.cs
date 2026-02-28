using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Usuarios
{
    public partial class WindowCambiarPassword : Window
    {
        private Usuario _usuario;
        private UsuarioService _usuarioService = new UsuarioService();

        public WindowCambiarPassword(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            string pass = txtPassNueva.Password;
            string confirm = txtPassConfirmar.Password;

            // 1. Validaciones básicas
            if (string.IsNullOrWhiteSpace(pass) || pass.Length < 4)
            {
                MostrarError("La contraseña debe tener al menos 4 caracteres.");
                return;
            }

            if (pass != confirm)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }

            try
            {
                // 2. Actualizamos el objeto usuario
                _usuario.PasswordHash = pass;
                _usuario.DebeCambiarContrasena = false;

                // 3. Llamada a la capa de negocio
                bool exito = _usuarioService.ActualizarPassword(_usuario);

                if (exito)
                {
                    MessageBox.Show("Contraseña actualizada con éxito. ¡Bienvenido!", "Vetcare", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Indica que el proceso fue correcto
                    this.Close();
                }
                else
                {
                    MostrarError("No se pudo actualizar la contraseña. Inténtalo de nuevo.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error técnico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visibility = Visibility.Visible;
        }
    }
}