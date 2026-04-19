using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Utilidades;

namespace Vetcare.Presentacion.Usuarios
{
    /// <summary>
    /// Ventana encargada de gestionar el cambio de contraseña del usuario
    /// Puede funcionar en modo obligatorio (primer login) o voluntario (desde menú)
    /// </summary>
    public partial class WindowCambiarPassword : Window
    {
        // Usuario actual al que se le va a cambiar la contraseña
        private readonly Usuario _usuario;

        // Servicio para acceder a la lógica de negocio de usuarios
        private readonly UsuarioService _usuarioService = new();

        // Indica si es el primer login (cambio obligatorio)
        private readonly bool esPrimerLogin;

        // Patrón de seguridad de contraseña:
        // - Al menos una minúscula
        // - Al menos una mayúscula
        // - Al menos un número
        [System.Text.RegularExpressions.GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        /// <param name="usuario">Usuario actual</param>
        /// <param name="esPrimerLogin">Indica si es obligatorio cambiar contraseña</param>
        public WindowCambiarPassword(Usuario usuario, bool esPrimerLogin = false)
        {
            InitializeComponent();

            // Guardamos el usuario recibido
            this._usuario = usuario;

            // Indicamos el modo de uso
            this.esPrimerLogin = esPrimerLogin;

            // Configuramos la interfaz según el modo
            ConfigurarInterfaz();
        }

        /// <summary>
        /// Ajusta la UI dependiendo si es primer login o cambio desde menú
        /// </summary>
        private void ConfigurarInterfaz()
        {
            if (esPrimerLogin)
            {
                // MODO LOGIN
                txtTitulo.Text = "Cambio Obligatorio";
                txtSubtitulo.Text = "Tu cuenta requiere una nueva contraseña para activarse.";

                // Ocultamos campo de contraseña actual (No pedimos la contraseña actual)
                panelPassActual.Visibility = Visibility.Collapsed;

                // Ajustamos altura de la ventana
                this.Height = 450;
            }
            else
            {
                // MODO MENÚ: Cambio voluntario (requiere verificación)
                txtTitulo.Text = "Seguridad de Cuenta";
                txtSubtitulo.Text = "Introduce tu clave actual para poder establecer una nueva.";

                // Mostramos campo de contraseña actual
                panelPassActual.Visibility = Visibility.Visible;

                // Ajustamos altura
                this.Height = 520;
            }
        }

        /// <summary>
        /// Evento al hacer clic en "Actualizar contraseña"
        /// </summary>
        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            // Ocultamos errores previos
            brdError.Visibility = Visibility.Collapsed;

            // Si NO es primer login, validamos contraseña actual
            if (!esPrimerLogin)
            {
                // Generamos hash de la contraseña introducida
                string hashActual = Seguridad.Encriptar(txtPassActual.Password, _usuario.Salt!);

                // Comparamos con la almacenada
                if (hashActual != _usuario.PasswordHash)
                {
                    MostrarError("La contraseña actual no es correcta.");
                    return;
                }
            }

            // Validación de la nueva contraseña
            string nueva = txtPassNueva.Password;
            string confirm = txtPassConfirmar.Password;

            // Validación de seguridad mínima
            if (nueva.Length < 8 || !MyRegex().IsMatch(nueva))
            {
                MostrarError("Mínimo 8 caracteres, una mayúscula, una minúscula y un número.");
                return;
            }

            // Validar coincidencia
            if (nueva != confirm)
            {
                MostrarError("Las nuevas contraseñas no coinciden.");
                return;
            }

            // Guardado
            try
            {
                // Generamos un nuevo salt para mayor seguridad
                string nuevoSalt = Guid.NewGuid().ToString();

                // Encriptamos la nueva contraseña
                _usuario.PasswordHash = Seguridad.Encriptar(nueva, nuevoSalt);
                _usuario.Salt = nuevoSalt;

                // Indicamos que ya no necesita cambiar contraseña
                _usuario.DebeCambiarContrasena = false;

                // Guardamos en base de datos
                bool actualizado = _usuarioService.Actualizar(_usuario);

                if (actualizado)
                {
                    // Mensaje de éxito
                    MessageBox.Show("Contraseña actualizada correctamente.", "Contraseña actualizada", 
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    if (esPrimerLogin)
                    {
                        // Si es primer login, abrimos la ventana principal
                        MainWindow main = new(_usuario);
                        main.Show();

                        // Cambiamos la ventana principal de la aplicación
                        Application.Current.MainWindow = main;

                        // Cerramos esta ventana
                        this.Close();
                    }
                    else
                    {
                        // Si es cambio normal desde menú
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
                MostrarError("Error al procesar la solicitud: " + ex.Message);
            }
        }

        /// <summary>
        /// Muestra un mensaje de error en la interfaz
        /// </summary>
        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            brdError.Visibility = Visibility.Visible;
        }
    }
}