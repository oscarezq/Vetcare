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
        private bool esPrimerLogin;

        // El segundo parámetro decide si pedimos la pass actual o no
        public WindowCambiarPassword(Usuario usuario, bool esPrimerLogin = false)
        {
            InitializeComponent();
            this._usuario = usuario;
            this.esPrimerLogin = esPrimerLogin;

            ConfigurarInterfaz();
        }

        private void ConfigurarInterfaz()
        {
            if (esPrimerLogin)
            {
                // MODO LOGIN: No pedimos la actual, solo la nueva
                txtTitulo.Text = "Cambio Obligatorio";
                txtSubtitulo.Text = "Tu cuenta requiere una nueva contraseña para activarse.";
                panelPassActual.Visibility = Visibility.Collapsed;
                this.Height = 450;
            }
            else
            {
                // MODO MENÚ: Cambio voluntario, hay que confirmar identidad
                txtTitulo.Text = "Seguridad de Cuenta";
                txtSubtitulo.Text = "Introduce tu clave actual para poder establecer una nueva.";
                panelPassActual.Visibility = Visibility.Visible;
                this.Height = 520;
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            brdError.Visibility = Visibility.Collapsed;

            // --- 1. VALIDACIÓN SEGÚN EL MODO ---
            if (!esPrimerLogin)
            {
                // Si NO es el primer login, comparamos la actual con la de la BD
                string hashActual = Seguridad.Encriptar(txtPassActual.Password, _usuario.Salt);

                if (hashActual != _usuario.PasswordHash)
                {
                    MostrarError("La contraseña actual no es correcta.");
                    return;
                }
            }

            // --- 2. VALIDACIÓN DE LA NUEVA ---
            string nueva = txtPassNueva.Password;
            string confirm = txtPassConfirmar.Password;

            if (nueva.Length < 8 || !ValidarSeguridad(nueva))
            {
                MostrarError("Mínimo 8 caracteres, una mayúscula, una minúscula y un número.");
                return;
            }

            if (nueva != confirm)
            {
                MostrarError("Las nuevas contraseñas no coinciden.");
                return;
            }

            // --- 3. GUARDADO ---
            try
            {
                // Generamos seguridad fresca
                string nuevoSalt = Guid.NewGuid().ToString();
                _usuario.PasswordHash = Seguridad.Encriptar(nueva, nuevoSalt);
                _usuario.Salt = nuevoSalt;
                _usuario.DebeCambiarContrasena = false;

                // Guardamos en BD
                bool actualizado = _usuarioService.Actualizar(_usuario);

                if (actualizado)
                {
                    MessageBox.Show("Contraseña actualizada correctamente.", "VetCare", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (esPrimerLogin)
                    {
                        // Creamos la ventana principal
                        MainWindow main = new MainWindow(_usuario);
                        main.Show();

                        // IMPORTANTE: Marcamos la ventana actual para cerrar sin error de DialogResult
                        Application.Current.MainWindow = main; // Cambiamos la referencia principal de la app
                        this.Close();
                    }
                    else
                    {
                        // Si es cambio voluntario desde el menú
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                else
                {
                    MostrarError("No se pudo actualizar en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                // Loguea el error real en la consola de depuración para saber qué pasó exactamente
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
                MostrarError("Error al procesar la solicitud: " + ex.Message);
            }
        }

        private bool ValidarSeguridad(string pass)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(pass, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$");
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            brdError.Visibility = Visibility.Visible;
        }
    }
}