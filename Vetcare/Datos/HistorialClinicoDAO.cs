
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad HistorialClinico.
    /// Gestiona las operaciones de inserción, consulta, actualización y eliminación
    /// del historial clínico de las mascotas en la base de datos.
    /// </summary>
    public class HistorialClinicoDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        readonly Conexion conexion = new();

        /// <summary>
        /// Inserta un nuevo registro de historial clínico en la base de datos.
        /// </summary>
        /// <param name="h">Objeto HistorialClinico a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(HistorialClinico h)
        {
            using MySqlConnection conn = conexion.ObtenerConexion();
            conn.Open();

            string sql = @"INSERT INTO historial_clinico (id_mascota, id_cita, id_veterinario, 
                               peso, diagnostico, tratamiento, observaciones)
                           VALUES (@idMascota, @idCita, @idVeterinario, @peso, @diagnostico, 
                               @tratamiento, @observaciones)";

            MySqlCommand cmd = new(sql, conn);
            CargarParametros(cmd, h);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Obtiene un historial clínico asociado a una cita.
        /// </summary>
        /// <param name="idCita">Identificador de la cita.</param>
        /// <returns>Objeto HistorialClinico si existe; en caso contrario, null.</returns>
        public HistorialClinico? ObtenerPorIdCita(int idCita)
        {
            HistorialClinico? historial = null;

            using MySqlConnection conn = conexion.ObtenerConexion();
            conn.Open();

            string sql = @"SELECT * 
                           FROM historial_clinico
                           WHERE id_cita=@id";

            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@id", idCita);

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
                historial = MappingHistorial(reader);

            return historial;
        }

        /// <summary>
        /// Obtiene el historial clínico de una mascota específica.
        /// </summary>
        /// <param name="idMascota">Identificador de la mascota.</param>
        /// <returns>Lista de historiales clínicos.</returns>
        public List<HistorialClinico> ObtenerPorMascota(int idMascota)
        {
            List<HistorialClinico> lista = new();

            using MySqlConnection conn = conexion.ObtenerConexion();

            string sql = @"SELECT h.*, 
                                  CONCAT(u.nombre, ' ', u.apellidos) AS nombre_veterinario, 
                                  c.motivo AS motivo_cita
                           FROM historial_clinico h INNER JOIN veterinarios v 
                               ON h.id_veterinario = v.id_veterinario
                               INNER JOIN usuarios u ON v.id_usuario = u.id_usuario 
                               LEFT JOIN citas c ON h.id_cita = c.id_cita
                           WHERE h.id_mascota = @idMascota
                           ORDER BY h.fecha_hora DESC";

            MySqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@idMascota", idMascota);

            conn.Open();

            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                HistorialClinico historial = MappingHistorial(reader);
                historial.NombreVeterinario = reader["nombre_veterinario"].ToString();
                historial.Motivo = reader["motivo_cita"].ToString();

                lista.Add(historial);
            }

            return lista;
        }

        /// <summary>
        /// Actualiza los datos de un historial clínico existente.
        /// </summary>
        /// <param name="h">Objeto con los datos actualizados.</param>
        /// <returns>True si la actualización se realiza correctamente.</returns>
        public bool Actualizar(HistorialClinico h)
        {
            using MySqlConnection conn = conexion.ObtenerConexion();
            conn.Open();

            string sql = @"UPDATE historial_clinico 
                           SET peso=@peso,
                               diagnostico=@diagnostico,
                               tratamiento=@tratamiento,
                               observaciones=@observaciones
                           WHERE id_historial=@id";

            MySqlCommand cmd = new(sql, conn);

            cmd.Parameters.AddWithValue("@peso", h.Peso);
            cmd.Parameters.AddWithValue("@diagnostico", h.Diagnostico);
            cmd.Parameters.AddWithValue("@tratamiento", h.Tratamiento);
            cmd.Parameters.AddWithValue("@observaciones", h.Observaciones);
            cmd.Parameters.AddWithValue("@id", h.IdHistorial);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto HistorialClinico.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="h">Objeto historial clínico.</param>
        private static void CargarParametros(MySqlCommand cmd, HistorialClinico h)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idMascota", h.IdMascota);
            cmd.Parameters.AddWithValue("@idCita", h.IdCita as object ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idVeterinario", h.IdVeterinario);
            cmd.Parameters.AddWithValue("@peso", h.Peso);
            cmd.Parameters.AddWithValue("@diagnostico", h.Diagnostico);
            cmd.Parameters.AddWithValue("@tratamiento", h.Tratamiento);
            cmd.Parameters.AddWithValue("@observaciones", h.Observaciones);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto HistorialClinico.
        /// </summary>
        /// <param name="reader">Lector de datos.</param>
        /// <returns>Objeto HistorialClinico.</returns>
        private static HistorialClinico MappingHistorial(MySqlDataReader reader)
        {
            return new HistorialClinico
            {
                IdHistorial = Convert.ToInt32(reader["id_historial"]),
                IdMascota = Convert.ToInt32(reader["id_mascota"]),
                IdVeterinario = Convert.ToInt32(reader["id_veterinario"]),
                IdCita = reader["id_cita"] != DBNull.Value ? Convert.ToInt32(reader["id_cita"]) : (int?)null,
                FechaHora = Convert.ToDateTime(reader["fecha_hora"]),
                Peso = reader["peso"] != DBNull.Value ? Convert.ToDecimal(reader["peso"]) : 0,
                Diagnostico = reader["diagnostico"].ToString(),
                Tratamiento = reader["tratamiento"].ToString(),
                Observaciones = reader["observaciones"].ToString(),
            };
        }
    }
}