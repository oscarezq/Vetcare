using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada de realizar las operaciones de acceso a datos relacionadas con 
    /// la entidad Usuario en la base de datos vetcare usando MySQL.
    /// </summary>
    public class UsuarioDAO
    {
        private Conexion conexion = new Conexion();

        /// <summary>
        /// Valida las credenciales de un usuario para el inicio de sesión.
        /// </summary>
        public Usuario Login(string user, string pass)
        {
            Usuario objetoUsuario = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                try
                {
                    con.Open();
                    string query = @"
                        SELECT u.id_usuario, u.username, u.password_hash, u.salt, u.nombre, 
                               u.apellidos, u.email, u.telefono, u.id_rol, u.activo, u.debe_cambiar_password, r.nombre as nombre_rol
                        FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol
                        WHERE u.username = @user AND u.activo = 1";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", user);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string hashGuardado = dr["password_hash"].ToString();
                            string salt = dr["salt"].ToString();
                            string hashIntroducido = Seguridad.Encriptar(pass, salt);

                            if (hashGuardado == hashIntroducido)
                            {
                                objetoUsuario = MapearUsuarioCompleto(dr);
                            }
                        }
                    }
                }
                catch (Exception ex) { 
                    throw new Exception("Error en la autenticación: " + ex.Message); 
                }
            }
            return objetoUsuario;
        }

        // --- CONSULTAS ---

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new List<Usuario>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"SELECT u.*, r.nombre as nombre_rol 
                               FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read()) lista.Add(MapearUsuarioCompleto(dr));
                    }
                }
                catch (Exception ex) { throw new Exception("Error al obtener usuarios: " + ex.Message); }
            }
            return lista;
        }

        public Usuario ObtenerPorId(int id)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT u.*, r.nombre as nombre_rol FROM usuarios u INNER JOIN roles r ON u.id_rol = r.id_rol WHERE u.id_usuario = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read()) return MapearUsuarioCompleto(dr);
                    }
                }
                catch (Exception ex) { throw new Exception("Error al obtener usuario: " + ex.Message); }
            }
            return null;
        }

        // --- INSERCIONES ---

        public int Insertar(Usuario u)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"INSERT INTO usuarios 
                       (username, password_hash, salt, nombre, apellidos, email, telefono, id_rol, activo) 
                       VALUES (@user, @hash, @salt, @nom, @ape, @mail, @tel, @rol, 1)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
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
        }

        public bool InsertarVarios(List<Usuario> usuarios)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction tra = con.BeginTransaction())
                {
                    try
                    {
                        foreach (var u in usuarios)
                        {
                            string sql = @"INSERT INTO usuarios (username, password_hash, salt, nombre, apellidos, email, telefono, id_rol, activo, debe_cambiar_password) 
                                           VALUES (@user, @hash, @salt, @nom, @ape, @mail, @tel, @rol, 1, 1)";
                            MySqlCommand cmd = new MySqlCommand(sql, con, tra);
                            CargarParametros(cmd, u);
                            cmd.ExecuteNonQuery();
                        }
                        tra.Commit(); return true;
                    }
                    catch (Exception ex) { tra.Rollback(); throw new Exception("Error en inserción masiva: " + ex.Message); }
                }
            }
        }

        public bool Actualizar(Usuario u)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Añadimos password_hash, salt y debe_cambiar_password a la consulta
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

                MySqlCommand cmd = new MySqlCommand(sql, con);

                // Cargamos los parámetros básicos usando tu método auxiliar
                CargarParametros(cmd, u);

                // Añadimos los parámetros específicos que faltaban o son para el WHERE
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
        }

        // --- BORRADOS (LÓGICOS) ---

        public bool BorradoLogico(int id)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "UPDATE usuarios SET activo = 0 WHERE id_usuario = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);
                try { con.Open(); return cmd.ExecuteNonQuery() > 0; }
                catch (Exception ex) { throw new Exception("Error en borrado lógico: " + ex.Message); }
            }
        }

        public bool BorradoLogicoVarios(List<int> ids)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction tra = con.BeginTransaction())
                {
                    try
                    {
                        foreach (int id in ids)
                        {
                            string sql = "UPDATE usuarios SET activo = 0 WHERE id_usuario = @id";
                            MySqlCommand cmd = new MySqlCommand(sql, con, tra);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                        tra.Commit(); return true;
                    }
                    catch (Exception ex) { tra.Rollback(); throw new Exception("Error en borrado masivo: " + ex.Message); }
                }
            }
        }

        public bool ExisteUsername(string username)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                // Usamos COUNT para que sea una consulta ligera
                string sql = "SELECT COUNT(*) FROM usuarios WHERE username = @user";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@user", username);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // --- MÉTODOS AUXILIARES ---

        private void CargarParametros(MySqlCommand cmd, Usuario u)
        {
            // Es buena práctica limpiar para evitar errores de parámetros duplicados
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

        private Usuario MapearUsuarioCompleto(MySqlDataReader dr)
        {
            return new Usuario
            {
                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                Username = dr["username"].ToString(),
                IdRol = Convert.ToInt32(dr["id_rol"]),
                NombreRol = dr["nombre_rol"].ToString(),
                Nombre = dr["nombre"].ToString(),
                Apellidos = dr["apellidos"].ToString(),
                Email = dr["email"] != DBNull.Value ? dr["email"].ToString() : "",
                Telefono = dr["telefono"] != DBNull.Value ? dr["telefono"].ToString() : "",
                Activo = dr["activo"] != DBNull.Value && Convert.ToBoolean(dr["activo"]),
                FechaAlta = dr["fecha_alta"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_alta"]) : DateTime.MinValue,
                DebeCambiarContrasena = dr["debe_cambiar_password"] != DBNull.Value && Convert.ToBoolean(dr["debe_cambiar_password"])
            };
        }
    }
}