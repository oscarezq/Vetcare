using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada del acceso a datos para la entidad Veterinario.
    /// Gestiona operaciones CRUD y operaciones masivas contra la base de datos MySQL.
    /// </summary>
    public class VeterinarioDAO
    {
        /// <summary>
        /// Cadena de conexión a la base de datos.
        /// (Idealmente debería centralizarse en una clase Conexion).
        /// </summary>
        private string cadenaConexion = "Server=localhost;Database=vetcare;Uid=root;Pwd=;";

        /// <summary>
        /// Obtiene todos los veterinarios activos realizando un JOIN con la tabla usuarios.
        /// </summary>
        /// <returns>Lista de veterinarios activos.</returns>
        public List<Veterinario> ObtenerTodos()
        {
            List<Veterinario> lista = new List<Veterinario>();

            string sql = @"SELECT v.id_veterinario, v.id_usuario, v.especialidad, v.numero_colegiado, u.username, u.nombre, u.apellidos 
                            FROM veterinarios v
                            INNER JOIN usuarios u ON v.id_usuario = u.id_usuario";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Veterinario
                            {
                                IdVeterinario = Convert.ToInt32(dr["id_veterinario"]),
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Especialidad = dr["especialidad"].ToString(),
                                NumeroColegiado = dr["numero_colegiado"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Apellidos = dr["apellidos"].ToString(),
                                Username = dr["username"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.ObtenerTodos: " + ex.Message);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un veterinario activo por su ID.
        /// </summary>
        /// <param name="idVeterinario">ID del veterinario.</param>
        /// <returns>Objeto Veterinario si existe, null si no se encuentra.</returns>
        public Veterinario ObtenerPorId(int idVeterinario)
        {
            Veterinario veterinario = null;

            string sql = @"SELECT v.id_veterinario, v.id_usuario, v.especialidad, v.numero_colegiado,
                          u.nombre, u.apellidos
                   FROM veterinarios v
                   INNER JOIN usuarios u ON v.id_usuario = u.id_usuario
                   WHERE v.id_veterinario = @idVeterinario
                   AND u.activo = TRUE";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@idVeterinario", idVeterinario);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            veterinario = new Veterinario
                            {
                                IdVeterinario = Convert.ToInt32(dr["id_veterinario"]),
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Especialidad = dr["especialidad"].ToString(),
                                NumeroColegiado = dr["numero_colegiado"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Apellidos = dr["apellidos"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.ObtenerPorId: " + ex.Message);
            }

            return veterinario;
        }

        /// <summary>
        /// Obtiene los datos profesionales de un veterinario usando el ID de su cuenta de usuario.
        /// </summary>
        public Veterinario ObtenerPorIdUsuario(int idUsuario)
        {
            Veterinario veterinario = null;

            // Cambiamos el WHERE para buscar por v.id_usuario en lugar de v.id_veterinario
            string sql = @"SELECT v.id_veterinario, v.id_usuario, v.especialidad, v.numero_colegiado,
                           u.nombre, u.apellidos
                    FROM veterinarios v
                    INNER JOIN usuarios u ON v.id_usuario = u.id_usuario
                    WHERE v.id_usuario = @idUsuario
                    AND u.activo = TRUE";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            veterinario = new Veterinario
                            {
                                IdVeterinario = Convert.ToInt32(dr["id_veterinario"]),
                                IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                                Especialidad = dr["especialidad"].ToString(),
                                NumeroColegiado = dr["numero_colegiado"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Apellidos = dr["apellidos"].ToString()
                            };
                        }
                    }
                }
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
        /// <returns>True si la operación fue exitosa.</returns>
        public bool Insertar(Veterinario v)
        {
            string sql = @"INSERT INTO veterinarios (id_usuario, especialidad, numero_colegiado)
                           VALUES (@idUsuario, @especialidad, @numeroColegiado)";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@idUsuario", v.IdUsuario);
                    cmd.Parameters.AddWithValue("@especialidad", v.Especialidad);
                    cmd.Parameters.AddWithValue("@numeroColegiado", v.NumeroColegiado);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.Insertar: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserta múltiples veterinarios utilizando una transacción.
        /// </summary>
        /// <param name="lista">Lista de veterinarios a insertar.</param>
        /// <returns>True si todos se insertaron correctamente.</returns>
        public bool InsertarVarios(List<Veterinario> lista)
        {
            using (MySqlConnection con = new MySqlConnection(cadenaConexion))
            {
                con.Open();
                MySqlTransaction trans = con.BeginTransaction();

                try
                {
                    foreach (var v in lista)
                    {
                        string sql = @"INSERT INTO veterinarios (id_usuario, especialidad, numero_colegiado)
                                       VALUES (@idUsuario, @especialidad, @numeroColegiado)";

                        MySqlCommand cmd = new MySqlCommand(sql, con, trans);
                        cmd.Parameters.AddWithValue("@idUsuario", v.IdUsuario);
                        cmd.Parameters.AddWithValue("@especialidad", v.Especialidad);
                        cmd.Parameters.AddWithValue("@numeroColegiado", v.NumeroColegiado);

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Actualiza los datos de un veterinario existente.
        /// </summary>
        /// <param name="v">Objeto Veterinario con los nuevos datos.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        public bool Actualizar(Veterinario v)
        {
            string sql = @"UPDATE veterinarios 
                           SET especialidad = @especialidad,
                               numero_colegiado = @numeroColegiado
                           WHERE id_veterinario = @idVeterinario";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@especialidad", v.Especialidad);
                    cmd.Parameters.AddWithValue("@numeroColegiado", v.NumeroColegiado);
                    cmd.Parameters.AddWithValue("@idVeterinario", v.IdVeterinario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.Actualizar: " + ex.Message);
            }
        }

        /// <summary>
        /// Actualiza múltiples veterinarios dentro de una transacción.
        /// </summary>
        /// <param name="lista">Lista de veterinarios a actualizar.</param>
        /// <returns>True si todas las actualizaciones fueron exitosas.</returns>
        public bool ActualizarVarios(List<Veterinario> lista)
        {
            using (MySqlConnection con = new MySqlConnection(cadenaConexion))
            {
                con.Open();
                MySqlTransaction trans = con.BeginTransaction();

                try
                {
                    foreach (var v in lista)
                    {
                        string sql = @"UPDATE veterinarios 
                                       SET especialidad = @especialidad,
                                           numero_colegiado = @numeroColegiado
                                       WHERE id_veterinario = @idVeterinario";

                        MySqlCommand cmd = new MySqlCommand(sql, con, trans);
                        cmd.Parameters.AddWithValue("@especialidad", v.Especialidad);
                        cmd.Parameters.AddWithValue("@numeroColegiado", v.NumeroColegiado);
                        cmd.Parameters.AddWithValue("@idVeterinario", v.IdVeterinario);

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Realiza un borrado lógico del veterinario desactivando su usuario asociado.
        /// </summary>
        /// <param name="idVeterinario">ID del veterinario a desactivar.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        public bool BorradoLogico(int idVeterinario)
        {
            string sql = @"UPDATE usuarios u
                           INNER JOIN veterinarios v ON u.id_usuario = v.id_usuario
                           SET u.activo = FALSE
                           WHERE v.id_veterinario = @idVeterinario";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@idVeterinario", idVeterinario);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en VeterinarioDAO.BorradoLogico: " + ex.Message);
            }
        }

        /// <summary>
        /// Realiza el borrado lógico de múltiples veterinarios usando transacción.
        /// </summary>
        /// <param name="ids">Lista de IDs de veterinarios a desactivar.</param>
        /// <returns>True si todos fueron desactivados correctamente.</returns>
        public bool BorradoLogicoVarios(List<int> ids)
        {
            using (MySqlConnection con = new MySqlConnection(cadenaConexion))
            {
                con.Open();
                MySqlTransaction trans = con.BeginTransaction();

                try
                {
                    foreach (int id in ids)
                    {
                        string sql = @"UPDATE usuarios u
                                       INNER JOIN veterinarios v ON u.id_usuario = v.id_usuario
                                       SET u.activo = FALSE
                                       WHERE v.id_veterinario = @idVeterinario";

                        MySqlCommand cmd = new MySqlCommand(sql, con, trans);
                        cmd.Parameters.AddWithValue("@idVeterinario", id);

                        cmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Busca el ID de la tabla veterinarios que corresponde a un ID de la tabla usuarios.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario logueado.</param>
        /// <returns>El int id_veterinario encontrado, o 0 si no existe.</returns>
        public int ObtenerIdVeterinarioPorUsuario(int idUsuario)
        {
            int idEncontrado = 0;
            string sql = "SELECT id_veterinario FROM veterinarios WHERE id_usuario = @idUsuario";

            try
            {
                using (MySqlConnection con = new MySqlConnection(cadenaConexion))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                        // ExecuteScalar es perfecto aquí porque solo queremos UNA columna de UNA fila (un int)
                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null && resultado != DBNull.Value)
                        {
                            idEncontrado = Convert.ToInt32(resultado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener ID de veterinario: " + ex.Message);
            }

            return idEncontrado;
        }
    }
}