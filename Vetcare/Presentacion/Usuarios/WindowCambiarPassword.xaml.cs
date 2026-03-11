using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Utilidades;

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
            if (string.IsNullOrWhiteSpace(pass) || pass.Length < 8)
            {
                MostrarError("La contraseña debe tener al menos 8 caracteres.");
                return;
            }

            // Validación de seguridad: mayúscula, minúscula, número, carácter especial
            if (!EsContrasenaSegura(pass))
            {
                MostrarError("La contraseña debe contener al menos una mayúscula, una minúscula y un número.");
                return;
            }

            if (pass != confirm)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }

            try
            {
                // Generamos un nuevo Salt aleatorio para esta nueva contraseña
                string nuevoSalt = Guid.NewGuid().ToString();

                // Encriptamos la contraseña usando tu clase de utilidad
                string hashEncriptado = Seguridad.Encriptar(pass, nuevoSalt);

                // Actualizamos el objeto con los datos encriptados
                _usuario.PasswordHash = hashEncriptado;
                _usuario.Salt = nuevoSalt;
                _usuario.DebeCambiarContrasena = false;

                // 3. Llamada a la capa de negocio
                bool exito = _usuarioService.Actualizar(_usuario);

                if (exito)
                {
                    MessageBox.Show("Contraseña actualizada con éxito. ¡Bienvenido!", "Vetcare", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow main = new MainWindow(_usuario);
                    main.Show();
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

        // Método auxiliar para validar seguridad de contraseña
        private bool EsContrasenaSegura(string contrasena)
        {
            // Al menos una mayúscula, una minúscula y un número
            return System.Text.RegularExpressions.Regex.IsMatch(contrasena, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visibility = Visibility.Visible;
        }
    }
}