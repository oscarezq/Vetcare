using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Veterinario.
    /// Gestiona las operaciones de consulta, inserción, actualización y borrado lógico
    /// de veterinarios en la base de datos.
    /// </summary>
    public class VeterinarioDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todos los veterinarios del sistema junto con los datos del usuario asociado.
        /// </summary>
        /// <returns>Lista de veterinarios.</returns>
        public List<Veterinario> ObtenerTodos()
        {
            List<Veterinario> lista = new();

            string sql = @"SELECT v.id_veterinario, 
                                  v.id_usuario, 
                                  v.especialidad, 
                                  v.numero_colegiado, 
                                  u.username,
                                  u.nombre, 
                                  u.apellidos 
                           FROM veterinarios v
                           INNER JOIN usuarios u ON v.id_usuario = u.id_usuario";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);

                using MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                    lista.Add(MappingVeterinario(dr));
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.ObtenerTodos: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un veterinario a partir del ID de usuario asociado.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>Objeto Veterinario si existe; en caso contrario, null.</returns>
        public Veterinario? ObtenerPorIdUsuario(int idUsuario)
        {
            Veterinario? veterinario = null;

            string sql = @"SELECT v.id_veterinario, 
                                  v.id_usuario, 
                                  v.especialidad, 
                                  v.numero_colegiado,
                                  u.nombre, 
                                  u.apellidos
                           FROM veterinarios v
                           INNER JOIN usuarios u ON v.id_usuario = u.id_usuario
                           WHERE v.id_usuario = @idUsuario";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                using MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                    veterinario = MappingVeterinario(dr);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.ObtenerPorIdUsuario: " + ex.Message);
            }

            return veterinario;
        }

        /// <summary>
        /// Inserta un nuevo veterinario en la base de datos.
        /// </summary>
        /// <param name="v">Objeto Veterinario a insertar.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool Insertar(Veterinario v)
        {
            string sql = @"INSERT INTO veterinarios (id_usuario, especialidad, numero_colegiado)
                           VALUES (@idUsuario, @especialidad, @numeroColegiado)";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);
                CargarParametros(cmd, v);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.Insertar: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza los datos de un veterinario existente.
        /// </summary>
        public bool Actualizar(Veterinario v)
        {
            string sql = @"UPDATE veterinarios 
                           SET especialidad = @especialidad,
                               numero_colegiado = @numeroColegiado
                           WHERE id_veterinario = @idVeterinario";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);
                CargarParametros(cmd, v);
                cmd.Parameters.AddWithValue("@idVeterinario", v.IdVeterinario);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.Actualizar: " + ex.Message);
            }
        }

        /// <summary>
        /// Desactiva un veterinario mediante el usuario asociado.
        /// </summary>
        public bool BorradoLogico(int idVeterinario)
        {
            string sql = @"UPDATE usuarios u
                           INNER JOIN veterinarios v ON u.id_usuario = v.id_usuario
                           SET u.activo = FALSE
                           WHERE v.id_veterinario = @idVeterinario";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idVeterinario", idVeterinario);

                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.BorradoLogico: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el ID de veterinario a partir del ID de usuario.
        /// </summary>
        public int ObtenerIdVeterinarioPorUsuario(int idUsuario)
        {
            int idEncontrado = 0;
            string sql = "SELECT id_veterinario " +
                         "FROM veterinarios " +
                         "WHERE id_usuario = @idUsuario";

            try
            {
                using MySqlConnection con = conexion.ObtenerConexion();
                con.Open();

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                object resultado = cmd.ExecuteScalar();

                if (resultado != null && resultado != DBNull.Value)
                    idEncontrado = Convert.ToInt32(resultado);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener ID de veterinario: " + ex.Message);
            }

            return idEncontrado;
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto Veterinario.
        /// </summary>
        private static void CargarParametros(MySqlCommand cmd, Veterinario v)
        {
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@idUsuario", v.IdUsuario);
            cmd.Parameters.AddWithValue("@especialidad", v.Especialidad);
            cmd.Parameters.AddWithValue("@numeroColegiado", v.NumeroColegiado);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Veterinario.
        /// </summary>
        private static Veterinario MappingVeterinario(MySqlDataReader dr)
        {
            return new Veterinario
            {
                IdVeterinario = Convert.ToInt32(dr["id_veterinario"]),
                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                Especialidad = dr["especialidad"].ToString(),
                NumeroColegiado = dr["numero_colegiado"].ToString(),
                Nombre = dr["nombre"]?.ToString(),
                Apellidos = dr["apellidos"]?.ToString(),
                Username = dr["username"] != DBNull.Value ? dr["username"].ToString() : ""
            };
        }
    }
}