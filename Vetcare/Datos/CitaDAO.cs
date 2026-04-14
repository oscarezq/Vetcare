using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Cita.
    /// Gestiona las operaciones de consulta, inserción, actualización y eliminación
    /// de citas en la base de datos.
    /// </summary>
    class CitaDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todas las citas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de citas.</returns>
        public List<Cita> ObtenerTodas()
        {
            List<Cita> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @" SELECT c.id_cita, 
                                       c.id_mascota, 
                                       c.id_veterinario, 
                                       c.fecha_hora, 
                                       c.duracion_estimada, 
                                       c.motivo, 
                                       c.estado, 
                                       c.observaciones,
                                       m.nombre AS nombre_mascota, 
                                       CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno, 
                                       cli.id_cliente AS id_cliente_dueno, 
                                       CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario, 
                                       v.numero_colegiado, 
                                       v.id_usuario AS id_usuario_veterinario 
                                FROM citas c INNER JOIN mascotas m ON c.id_mascota = m.id_mascota 
                                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente 
                                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario 
                                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario 
                                ORDER BY c.fecha_hora ASC";

                MySqlCommand cmd = new(sql, con);
                using MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    lista.Add(MappingCita(rdr));
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene una cita por su identificador.
        /// </summary>
        /// <param name="idCita">Identificador de la cita.</param>
        /// <returns>Objeto Cita si existe; en caso contrario, null.</returns>
        public Cita? ObtenerPorId(int idCita)
        {
            Cita? cita = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"SELECT c.id_cita, 
                                      c.id_mascota, 
                                      c.id_veterinario, 
                                      c.fecha_hora, 
                                      c.duracion_estimada, 
                                      c.motivo, 
                                      c.estado,
                                      c.observaciones, 
                                      m.nombre AS nombre_mascota, 
                                      CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno, 
                                      cli.id_cliente AS id_cliente_dueno, 
                                      CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario,
                                      v.numero_colegiado, 
                                      v.id_usuario AS id_usuario_veterinario 
                                FROM citas c INNER JOIN mascotas m ON c.id_mascota = m.id_mascota 
                                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente 
                                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario 
                                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario 
                                WHERE c.id_cita = @id"; ;

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@id", idCita);
                using MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    cita = MappingCita(rdr);
            }

            return cita;
        }

        /// <summary>
        /// Obtiene las citas del día actual para un veterinario específico.
        /// </summary>
        /// <param name="idVeterinario">Identificador del veterinario.</param>
        /// <returns>Lista de citas del día.</returns>
        public List<Cita> ObtenerCitasHoyPorVeterinario(int idVeterinario)
        {
            List<Cita> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @" SELECT c.id_cita, 
                                       c.id_mascota,
                                       c.id_veterinario, 
                                       c.fecha_hora, 
                                       c.duracion_estimada, 
                                       c.motivo, 
                                       c.estado, 
                                       c.observaciones, 
                                       m.nombre AS nombre_mascota, 
                                       cli.id_cliente AS id_cliente_dueno, 
                                       CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno, 
                                       CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario, 
                                       v.numero_colegiado,
                                       v.id_usuario AS id_usuario_veterinario 
                                FROM citas c INNER JOIN mascotas m ON c.id_mascota = m.id_mascota 
                                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente 
                                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario 
                                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario 
                                WHERE c.id_veterinario = @idVeterinario AND DATE(c.fecha_hora) = CURDATE() 
                                    AND c.estado <> 'Cancelada' ORDER BY c.fecha_hora ASC";

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idVeterinario", idVeterinario);
                using MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    lista.Add(MappingCita(rdr));
            }

            return lista;
        }

        /// <summary>
        /// Obtiene las próximas citas del día actual.
        /// </summary>
        /// <returns>Lista de citas ordenadas por fecha.</returns>
        public List<Cita> ObtenerProximasCitas()
        {
            List<Cita> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @" SELECT c.id_cita, 
                                       c.id_mascota, 
                                       c.id_veterinario, 
                                       c.fecha_hora, 
                                       c.duracion_estimada, 
                                       c.motivo, 
                                       c.estado, 
                                       c.observaciones, 
                                       m.nombre AS nombre_mascota, 
                                       CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno, 
                                       cli.id_cliente AS id_cliente_dueno, 
                                       CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario, 
                                       v.numero_colegiado, 
                                       v.id_usuario AS id_usuario_veterinario 
                                FROM citas c INNER JOIN mascotas m ON c.id_mascota = m.id_mascota 
                                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente 
                                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario 
                                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario 
                                WHERE DATE(c.fecha_hora) = CURDATE() AND c.estado <> 'Cancelada' 
                                ORDER BY c.fecha_hora ASC";

                MySqlCommand cmd = new(sql, con);
                using MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    lista.Add(MappingCita(rdr));
            }

            return lista;
        }

        /// <summary>
        /// Inserta una nueva cita en la base de datos.
        /// </summary>
        /// <param name="cita">Objeto cita a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(Cita cita)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @" INSERT INTO citas (id_mascota, id_veterinario, fecha_hora, duracion_estimada, 
                                motivo, estado, observaciones) 
                            VALUES (@idMascota, @idVeterinario, @fecha_hora, @duracion_estimada, @motivo,
                                estado, @observaciones)";

            MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, cita);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Actualiza los datos de una cita existente.
        /// </summary>
        /// <param name="cita">Objeto cita con los datos actualizados.</param>
        /// <returns>True si la actualización se realiza correctamente.</returns>
        public bool Actualizar(Cita cita)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @" UPDATE citas 
                            SET id_mascota = @idMascota, 
                                id_veterinario = @idVeterinario, 
                                fecha_hora = @fecha_hora, 
                                duracion_estimada = @duracion_estimada, 
                                motivo = @motivo, 
                                estado = @estado, 
                                observaciones = @observaciones
                            WHERE id_cita = @id";

            MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, cita);
            cmd.Parameters.AddWithValue("@id", cita.IdCita);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Actualiza únicamente el estado de una cita.
        /// </summary>
        /// <param name="idCita">Identificador de la cita.</param>
        /// <param name="estado">Nuevo estado de la cita.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool ActualizarEstado(int idCita, string estado)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @" UPDATE citas 
                            SET estado = @estado 
                            WHERE id_cita = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idCita);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Cuenta el número de citas programadas para el día actual.
        /// </summary>
        /// <returns>Número de citas de hoy.</returns>
        public int ContarCitasHoy()
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @"SELECT COUNT(*) 
                           FROM citas 
                           WHERE DATE(fecha_hora) = CURDATE() AND estado <> 'Cancelada'";

            MySqlCommand cmd = new(sql, con);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Cuenta el número de citas de hoy para un veterinario específico.
        /// </summary>
        /// <param name="idUsuarioVeterinario">Identificador del veterinario.</param>
        /// <returns>Número de citas.</returns>
        public int ContarCitasHoyPorVeterinario(int idUsuarioVeterinario)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @" SELECT COUNT(*) 
                            FROM citas c INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario 
                            WHERE c.id_veterinario = @idVeterinario AND DATE(c.fecha_hora) = CURDATE() 
                                AND c.estado <> 'Cancelada'";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@idVeterinario", idUsuarioVeterinario);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto Cita.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="cita">Objeto cita con los datos.</param>
        private static void CargarParametros(MySqlCommand cmd, Cita cita)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idMascota", cita.IdMascota);
            cmd.Parameters.AddWithValue("@idVeterinario", cita.IdVeterinario);
            cmd.Parameters.AddWithValue("@fecha_hora", cita.FechaHora);
            cmd.Parameters.AddWithValue("@duracion_estimada", cita.DuracionEstimada);
            cmd.Parameters.AddWithValue("@motivo", cita.Motivo as object ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@estado", cita.Estado);
            cmd.Parameters.AddWithValue("@observaciones", cita.Observaciones);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Cita.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Cita.</returns>
        private static Cita MappingCita(MySqlDataReader rdr)
        {
            return new Cita
            {
                IdCita = Convert.ToInt32(rdr["id_cita"]),
                IdMascota = Convert.ToInt32(rdr["id_mascota"]),
                IdVeterinario = Convert.ToInt32(rdr["id_veterinario"]),
                IdUsuarioVeterinario = Convert.ToInt32(rdr["id_usuario_veterinario"]),
                IdUsuarioDueno = Convert.ToInt32(rdr["id_cliente_dueno"]),
                FechaHora = Convert.ToDateTime(rdr["fecha_hora"]),
                DuracionEstimada = Convert.ToInt32(rdr["duracion_estimada"]),
                Motivo = rdr["motivo"].ToString(),
                Estado = rdr["estado"].ToString(),
                Observaciones = rdr["observaciones"].ToString(),
                NombreMascota = rdr["nombre_mascota"].ToString(),
                NombreDueno = rdr["nombre_dueno"].ToString(),
                NombreVeterinario = rdr["nombre_veterinario"].ToString(),
                NumeroColegiado = rdr["numero_colegiado"].ToString(),
            };
        }
    }
}