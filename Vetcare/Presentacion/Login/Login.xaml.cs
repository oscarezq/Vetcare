using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Utilidades;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        // Instanciamos el Servicio para usuarios
        private readonly UsuarioService _usuarioService = new();
        private readonly VeterinarioService _veterinarioService = new();

        // Objeto usuario que representa el usuario con el que se hace login
        private Usuario? usuarioLogueado;

        public Login()
        {
            InitializeComponent();

            CrearUsuarioAdminSiNoExiste();
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el usuario y contraseña escritos en los TextBox
            string username = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            // Comprobamos que no son nulos ninguno de los dos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Introduce usuario y contraseña", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Llamamos al método para validar las credenciales
            string mensajeError = _usuarioService.ValidarLogin(username, password, out usuarioLogueado);

            // Evaluamos la respuesta
            if (string.IsNullOrEmpty(mensajeError))
            {
                // Si el usuario es veterinario (2), guardamos su ID de veterinario
                if (usuarioLogueado != null && usuarioLogueado.IdRol == 2)
                    usuarioLogueado.IdVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuarioLogueado.IdUsuario);

                Sesion.UsuarioActual = usuarioLogueado;

                // Si el usuario debe cambiar la contraseña porque es su primer inicio, se muestra la ventana de cambiar contreaseña.
                // Si no tiene que cambiar la contraseña, se abre la ventana principal
                if (usuarioLogueado!.DebeCambiarContrasena)
                {
                    WindowCambiarPassword winCambio = new(usuarioLogueado, true);
                    winCambio.Show();
                    this.Close();
                } 
                else
                {
                    MainWindow main = new(usuarioLogueado);
                    main.Show();
                    this.Close();
                }

            }
            else
            {
                //Si ha habido respuesta: Error. Mostramos el mensaje de error
                txtPassword.Clear();
                txtPassword.Focus();
                MessageBox.Show(mensajeError, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Método que inserta en la base de datos el usuario Admin si no hay ninguno creado
        /// </summary>
        private void CrearUsuarioAdminSiNoExiste()
        {
            if (!_usuarioService.ComprobarHayUsuarios())
            {
                // Generar hash y salt
                string salt = Seguridad.GenerarSalt();
                string hash = Seguridad.Encriptar("admin", salt);

                _usuarioService.InsertarUsuarioAdmin(hash, salt);
            }
        }
    }
}
