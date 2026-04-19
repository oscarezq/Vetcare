using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Utilidades;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        // Servicio encargado de la lógica de usuarios
        private readonly UsuarioService _usuarioService = new();

        // Servicio encargado de operaciones relacionadas con veterinarios
        private readonly VeterinarioService _veterinarioService = new();

        // Usuario actualmente autenticado en el sistema
        private Usuario? usuarioLogueado;

        /// <summary>
        /// Constructor de la ventana de login
        /// Inicializa componentes y crea usuario admin si no existe
        /// </summary>
        public Login()
        {
            InitializeComponent();

            // Asegura que exista un usuario administrador en la base de datos
            CrearUsuarioAdminSiNoExiste();
        }

        /// <summary>
        /// Evento del botón "Entrar"
        /// Valida credenciales y abre la ventana correspondiente
        /// </summary>
        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener valores introducidos por el usuario
            string username = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            // Validación básica de campos vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Introduce usuario y contraseña", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar credenciales contra la base de datos
            string mensajeError = _usuarioService.ValidarLogin(username, password, out usuarioLogueado);

            // Si no hay error, el login es correcto
            if (string.IsNullOrEmpty(mensajeError))
            {
                // Si el usuario es veterinario (rol 2), obtener su ID de veterinario
                if (usuarioLogueado != null && usuarioLogueado.IdRol == 2)
                    usuarioLogueado.IdVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuarioLogueado.IdUsuario);

                // Guardar usuario en sesión global
                Sesion.UsuarioActual = usuarioLogueado;

                // Si el usuario debe cambiar contraseña (primer acceso)
                if (usuarioLogueado!.DebeCambiarContrasena)
                {
                    // Abrir ventana de cambio de contraseña obligatoria
                    WindowCambiarPassword winCambio = new(usuarioLogueado, true);
                    winCambio.Show();
                    this.Close();
                }
                else
                {
                    // Abrir ventana principal del sistema
                    MainWindow main = new(usuarioLogueado);
                    main.Show();
                    this.Close();
                }

            }
            else
            {
                // En caso de error de login, limpiar contraseña y mostrar mensaje
                txtPassword.Clear();
                txtPassword.Focus();
                MessageBox.Show(mensajeError, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Crea un usuario administrador por defecto si no existe ninguno en la base de datos
        /// </summary>
        private void CrearUsuarioAdminSiNoExiste()
        {
            // Comprobar si ya existen usuarios en el sistema
            if (!_usuarioService.ComprobarHayUsuarios())
            {
                // Generar salt para seguridad de contraseña
                string salt = Seguridad.GenerarSalt();

                // Encriptar contraseña por defecto "admin"
                string hash = Seguridad.Encriptar("admin", salt);

                // Insertar usuario administrador en la base de datos
                _usuarioService.InsertarUsuarioAdmin(hash, salt);
            }
        }
    }
}