using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos.DAOs
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Usuario.
    /// Gestiona las operaciones de autenticación, consulta, inserción, actualización
    /// y borrado lógico de usuarios en la base de datos.
    /// </summary>
    public class UsuarioDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Valida las credenciales de un usuario para el inicio de sesión.
        /// </summary>
        /// <param name="user">Nombre de usuario.</param>
        /// <param name="pass">Contraseña en texto plano.</param>
        /// <returns>Objeto Usuario si las credenciales son correctas; en caso contrario, null.</returns>
        public Usuario? Login(string user, string pass)
        {
            Usuario? objetoUsuario = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                try
                {
                    con.Open();

                    string query = @"
                        SELECT u.id_usuario, 
                               u.username, 
                               u.password_hash, 
                               u.salt, 
                               u.nombre, 
                               u.fecha_alta,
                               u.apellidos, 
                               u.email, 
                               u.telefono, 
                               u.id_rol, 
                               u.activo, 
                               u.debe_cambiar_password, 
                               r.nombre as nombre_rol
                        FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol
                        WHERE u.username = @user AND u.activo = 1";

                    MySqlCommand cmd = new(query, con);
                    cmd.Parameters.AddWithValue("@user", user);

                    using MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        string? hashGuardado = rdr["password_hash"].ToString();
                        string? salt = rdr["salt"].ToString();

                        if (hashGuardado != null && salt != null)
                        {
                            string hashIntroducido = Seguridad.Encriptar(pass, salt);

                            if (hashGuardado == hashIntroducido)
                            {
                                objetoUsuario = MappingUsuario(rdr);
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

        /// <summary>
        /// Obtiene todos los usuarios registrados en el sistema.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"SELECT u.id_usuario, 
                                      u.username, 
                                      u.password_hash, 
                                      u.salt, 
                                      u.nombre, 
                                      u.fecha_alta,
                                      u.apellidos, 
                                      u.email, 
                                      u.telefono, 
                                      u.id_rol, 
                                      u.activo, 
                                      u.debe_cambiar_password, 
                                      r.nombre as nombre_rol 
                               FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol";

                MySqlCommand cmd = new(sql, con);

                try
                {
                    con.Open();

                    using MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                        lista.Add(MappingUsuario(dr));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener usuarios: " + ex.Message);
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="id">Identificador del usuario.</param>
        /// <returns>Objeto Usuario si existe; en caso contrario, null.</returns>
        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;

            using MySqlConnection con = conexion.ObtenerConexion();
            string sql = @"SELECT u.id_usuario, 
                                  u.username, 
                                  u.password_hash, 
                                  u.salt, 
                                  u.nombre, 
                                  u.fecha_alta,
                                  u.apellidos, 
                                  u.email, 
                                  u.telefono, 
                                  u.id_rol, 
                                  u.activo, 
                                  u.debe_cambiar_password,
                                  r.nombre as nombre_rol 
                           FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol 
                           WHERE u.id_usuario = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                con.Open();

                using MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    usuario = MappingUsuario(dr);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuario: " + ex.Message);
            }

            return usuario;
        }

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="u">Objeto usuario a insertar.</param>
        /// <returns>Identificador del usuario insertado.</returns>
        public int Insertar(Usuario u)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            string sql = @"INSERT INTO usuarios (username, password_hash, salt, nombre, 
                              apellidos, email, telefono, id_rol, activo) 
                           VALUES (@user, @hash, @salt, @nom, @ape, @mail, @tel, @rol, 1)";

            MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, u);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                return (int)cmd.LastInsertedId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar usuario: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserta el usuario admin en la base de datos.
        /// </summary>
        /// <param name="hash">Hash de la contraseña.</param>
        /// <param name="salt">Salt de la contraseña.</param>
        /// <returns>True si se ha insertado el usuario correctamente.</returns>
        public bool InsertarUsuarioAdmin(string hash, string salt)
        {
            using MySqlConnection con = conexion.ObtenerConexion();

            string insertQuery = @"INSERT INTO usuarios (id_rol, username, password_hash, salt, nombre, apellidos, 
                                       email, telefono, activo, debe_cambiar_password) 
                                   VALUES (@rol, @user, @hash, @salt, @nombre, @apellidos, @email, @telefono, 1, 1);";

            MySqlCommand cmd = new(insertQuery, con);

            // Datos del admin
            cmd.Parameters.AddWithValue("@rol", 1);
            cmd.Parameters.AddWithValue("@user", "admin");
            cmd.Parameters.AddWithValue("@hash", hash);
            cmd.Parameters.AddWithValue("@salt", salt);
            cmd.Parameters.AddWithValue("@nombre", "Admin");
            cmd.Parameters.AddWithValue("@apellidos", "Sistema");
            cmd.Parameters.AddWithValue("@email", "admin@admin.com");
            cmd.Parameters.AddWithValue("@telefono", "123456789");

            try
            {
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar admin: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="u">Objeto usuario con los datos actualizados.</param>
        /// <returns>True si la actualización se realiza correctamente.</returns>
        public bool Actualizar(Usuario u)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            string sql = @"UPDATE usuarios SET 
                                  username = @user, 
                                  password_hash = @hash, 
                                  salt = @salt, 
                                  nombre = @nom, 
                                  apellidos = @ape, 
                                  email = @mail, 
                                  telefono = @tel, 
                                  id_rol = @rol,
                                  debe_cambiar_password = @debeCambiar
                           WHERE id_usuario = @id";

            MySqlCommand cmd = new(sql, con);

            CargarParametros(cmd, u);
            cmd.Parameters.AddWithValue("@id", u.IdUsuario);
            cmd.Parameters.AddWithValue("@debeCambiar", u.DebeCambiarContrasena ? 1 : 0);

            try
            {
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar usuario: " + ex.Message);
            }
        }

        /// <summary>
        /// Realiza el borrado lógico de un usuario (lo desactiva).
        /// </summary>
        public bool BorradoLogico(int id)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            string sql = @"UPDATE usuarios
                           SET activo = 0 
                           WHERE id_usuario = @id";
            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en borrado lógico: " + ex.Message);
            }
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        public bool Reactivar(int idUsuario)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"UPDATE usuarios 
                           SET activo = TRUE
                           WHERE id_usuario = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idUsuario);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Verifica si ya existe un nombre de usuario en el sistema.
        /// </summary>
        public bool ExisteUsername(string username)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT COUNT(*)
                           FROM usuarios 
                           WHERE username = @user";
            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@user", username);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public bool ComprobarHayUsuarios()
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT COUNT(*)
                           FROM usuarios";
            MySqlCommand cmd = new(sql, con);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto Usuario.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="cita">Objeto usuario con los datos.</param>
        private static void CargarParametros(MySqlCommand cmd, Usuario u)
        {
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@user", u.Username);
            cmd.Parameters.AddWithValue("@hash", u.PasswordHash);
            cmd.Parameters.AddWithValue("@salt", u.Salt);
            cmd.Parameters.AddWithValue("@nom", u.Nombre);
            cmd.Parameters.AddWithValue("@ape", u.Apellidos);
            cmd.Parameters.AddWithValue("@mail", u.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@tel", u.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@rol", u.IdRol);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Usuario.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Cita.</returns>
        private static Usuario MappingUsuario(MySqlDataReader rdr)
        {
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(rdr["id_usuario"]),
                Username = rdr["username"].ToString(),
                PasswordHash = rdr["password_hash"].ToString(),
                Salt = rdr["salt"].ToString(),
                IdRol = Convert.ToInt32(rdr["id_rol"]),
                NombreRol = rdr["nombre_rol"].ToString(),
                Nombre = rdr["nombre"].ToString(),
                Apellidos = rdr["apellidos"].ToString(),
                Email = rdr["email"] != DBNull.Value ? rdr["email"].ToString() : "",
                Telefono = rdr["telefono"] != DBNull.Value ? rdr["telefono"].ToString() : "",
                Activo = rdr["activo"] != DBNull.Value && Convert.ToBoolean(rdr["activo"]),
                FechaAlta = rdr["fecha_alta"] != DBNull.Value ? Convert.ToDateTime(rdr["fecha_alta"]) : DateTime.MinValue,
                DebeCambiarContrasena = rdr["debe_cambiar_password"] != DBNull.Value && Convert.ToBoolean(rdr["debe_cambiar_password"])
            };
        }
    }
}