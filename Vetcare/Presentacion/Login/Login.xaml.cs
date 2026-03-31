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
        private UsuarioService _usuarioService = new UsuarioService();
        private VeterinarioService _veterinarioService = new VeterinarioService();

        // Objeto usuario que representa el usuario con el que se hace login
        private Usuario usuarioLogueado;

        public Login()
        {
            InitializeComponent();
            CrearUsuarioAdminSiNoExiste();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el usuario y contraseña escritos en los TextBox
            string username = txtUsuario.Text.Trim();
            string password = txtPassword.Password;

            // Comprobamos que no son nulos ninguno de los dos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Debe introducir usuario y contraseña", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Llamaos al método para validar las credenciales
            string mensajeError = _usuarioService.ValidarLogin(username, password, out usuarioLogueado);

            // Evaluamos la respuesta
            if (string.IsNullOrEmpty(mensajeError))
            {
                

                if (usuarioLogueado != null && usuarioLogueado.IdRol == 2)
                {
                    // Buscas el ID y lo guardas en el objeto que irá a la sesión
                    usuarioLogueado.IdVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuarioLogueado.IdUsuario);
                }

                Sesion.UsuarioActual = usuarioLogueado;

                if (usuarioLogueado.DebeCambiarContrasena)
                {
                    WindowCambiarPassword winCambio = new WindowCambiarPassword(usuarioLogueado, true);
                    winCambio.Show();
                    this.Close();
                } 
                else
                {
                    MainWindow main = new MainWindow(usuarioLogueado);
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

        private void CrearUsuarioAdminSiNoExiste()
        {
            Conexion conexion = new Conexion();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // Comprobar si ya existen usuarios
                string checkQuery = "SELECT COUNT(*) FROM usuarios;";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                    return; // Si ya hay usuarios, no hacemos nada

                // Datos del admin
                int idRol = 1;
                string username = "admin";
                string password = "admin";
                string nombre = "Admin";
                string apellidos = "Sistema";
                string email = "admin@admin.com";
                string telefono = "123456789";

                // Generar seguridad
                string salt = Seguridad.GenerarSalt();
                string hash = Seguridad.Encriptar(password, salt);

                // Insertar usuario
                string insertQuery = @"INSERT INTO usuarios 
                (id_rol, username, password_hash, salt, nombre, apellidos, email, telefono, activo, debe_cambiar_password) 
                VALUES 
                (@rol, @user, @hash, @salt, @nombre, @apellidos, @email, @telefono, 1, 0);";

                MySqlCommand cmd = new MySqlCommand(insertQuery, con);

                cmd.Parameters.AddWithValue("@rol", idRol);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apellidos", apellidos);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@telefono", telefono);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
