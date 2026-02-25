using System;
using System.Data;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada de realizar las operaciones de acceso a datos relacionadas con 
    /// la entidad Usuario en la base de datos vetcare.
    /// </summary>
    public class UsuarioDAO
    {
        private Conexion conexion = new Conexion();

        /// <summary>
        /// Valida las credenciales de un usuario para el inicio de sesión.
        /// </summary>
        /// <param name="user">Nombre de usuario proporcionado.</param>
        /// <param name="pass">Contraseña en texto plano proporcionada.</param>
        /// <returns>Un objeto Usuario si las credenciales son correctas; de lo contrario, null.</returns>
        public Usuario Login(string user, string pass)
        {
            // Objeto que representa el usuario con el que hacemos login
            Usuario objetoUsuario = null;

            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                try
                {
                    // Abrimos la conexión
                    con.Open();

                    // Consulta
                    string query = @"
                        SELECT u.id_usuario, u.username, u.password_hash, u.salt, u.nombre, 
                               u.apellidos, u.email, u.telefono, u.id_rol, r.nombre as nombre_rol
                        FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol
                        WHERE u.username = @user AND u.activo = 1";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Extraemos los datos de seguridad de la BD
                            string hashGuardado = dr["password_hash"].ToString();
                            string salt = dr["salt"].ToString();

                            // Encriptamos la clave que acaba de escribir el usuario con el salt de la BD
                            string hashIntroducido = Seguridad.Encriptar(pass, salt);

                            // Si coinciden, el login es exitoso y rellenamos el objeto
                            if (hashGuardado == hashIntroducido)
                            {
                                objetoUsuario = new Usuario
                                {
                                    IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                    Username = dr["username"].ToString(),
                                    IdRol = Convert.ToInt32(dr["id_rol"]),
                                    NombreRol = dr["nombre_rol"].ToString(),
                                    Nombre = dr["nombre"].ToString(),
                                    Apellidos = dr["apellidos"].ToString(),
                                    Email = dr["email"] != DBNull.Value ? dr["email"].ToString() : "",
                                    Telefono = dr["telefono"] != DBNull.Value ? dr["telefono"].ToString() : ""
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en la autenticación: " + ex.Message);
                }
            }

            return objetoUsuario;
        }
    }
}