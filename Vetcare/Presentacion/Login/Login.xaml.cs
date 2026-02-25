using System;
using System.Windows;
using MySql.Data.MySqlClient;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
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

        // Objeto usuario que representa el usuario con el que se hace login
        private Usuario usuarioLogueado;

        public Login()
        {
            InitializeComponent();
            CrearUsuariosSiNoExisten();
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
                // Si no ha habido respuesta: Éxito. Entramos a la app
                MainWindow main = new MainWindow(usuarioLogueado);
                main.Show();
                this.Close();
            }
            else
            {
                //Si ha habido respuesta: Error. Mostramos el mensaje de error
                txtPassword.Clear();
                txtPassword.Focus();
                MessageBox.Show(mensajeError, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void CrearUsuariosSiNoExisten()
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

                // Crear ADMIN
                InsertarUsuario(con, 1, "admin", "admin", "Admin", "Sistema", "admin@admin.com", "123456789");

                // Crear OSCAR
                InsertarUsuario(con, 2, "oscar", "oscar", "Oscar", "Admin", "oscar@vetcare.com", "638216257");

                // Crear VANESA
                InsertarUsuario(con, 3, "vanesa", "vanesa", "Vanesa", "Gonzales", "van@gonz.com", "687849434");
            }
        }


        private void InsertarUsuario(MySqlConnection con, int idRol, string username, string password, string nombre, string apellidos, string email, string telefono)
        {
            string salt = Seguridad.GenerarSalt();
            string hash = Seguridad.Encriptar(password, salt);

            string insertQuery = @"INSERT INTO usuarios 
                           (id_rol, username, password_hash, salt, nombre, apellidos, email, telefono, activo) 
                           VALUES 
                           (@rol, @user, @hash, @salt, @nombre, @apellidos, @email, @telefono, 1);";

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
