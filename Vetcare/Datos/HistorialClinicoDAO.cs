using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.DAO
{
    public class HistorialClinicoDAO
    {
        Conexion conexion = new Conexion();

        // =========================
        // MAPPING
        // =========================

        private HistorialClinico Map(MySqlDataReader reader)
        {
            return new HistorialClinico
            {
                IdHistorial = Convert.ToInt32(reader["id_historial"]),
                IdMascota = Convert.ToInt32(reader["id_mascota"]),
                IdVeterinario = Convert.ToInt32(reader["id_veterinario"]),
                // id_cita puede ser null en tu BD, manejamos el nulo:
                IdCita = reader["id_cita"] != DBNull.Value ? Convert.ToInt32(reader["id_cita"]) : (int?)null,

                FechaHora = Convert.ToDateTime(reader["fecha_hora"]),
                Peso = reader["peso"] != DBNull.Value ? Convert.ToDecimal(reader["peso"]) : 0,
                Diagnostico = reader["diagnostico"].ToString(),
                Tratamiento = reader["tratamiento"].ToString(),
                Observaciones = reader["observaciones"].ToString(),
            };
        }

        // =========================
        // INSERTAR
        // =========================

        public bool Insertar(HistorialClinico h)
        {
            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = @"INSERT INTO historial_clinico
                            (id_mascota, id_cita, id_veterinario, peso, diagnostico, tratamiento, observaciones)
                            VALUES
                            (@idMascota, @idCita, @idVeterinario, @peso, @diagnostico, @tratamiento, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@idMascota", h.IdMascota);
                cmd.Parameters.AddWithValue("@idCita", h.IdCita);
                cmd.Parameters.AddWithValue("@idVeterinario", h.IdVeterinario);
                cmd.Parameters.AddWithValue("@peso", h.Peso);
                cmd.Parameters.AddWithValue("@diagnostico", h.Diagnostico);
                cmd.Parameters.AddWithValue("@tratamiento", h.Tratamiento);
                cmd.Parameters.AddWithValue("@observaciones", h.Observaciones);

                conn.Open();
                

                if (cmd.ExecuteNonQuery() > 0)
                {
                    return true;
                } else { 
                    return false; 
                }
            }
        }

        // =========================
        // OBTENER POR ID
        // =========================

        public HistorialClinico ObtenerPorId(int id)
        {
            HistorialClinico historial = null;

            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM historial_clinico WHERE id_historial=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    historial = Map(reader);
            }

            return historial;
        }

        public HistorialClinico ObtenerPorIdCita(int idCita)
        {
            HistorialClinico historial = null;

            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM historial_clinico WHERE id_cita=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", idCita);

                conn.Open();

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                    historial = Map(reader);
            }

            return historial;
        }

        // =========================
        // LISTAR TODO
        // =========================

        public List<HistorialClinico> ListarTodos()
        {
            List<HistorialClinico> lista = new List<HistorialClinico>();

            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM historial_clinico ORDER BY fecha_hora DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(Map(reader));
                }
            }

            return lista;
        }

        // =========================
        // LISTAR POR MASCOTA
        // =========================

        public List<HistorialClinico> ObtenerPorMascota(int idMascota)
        {
            List<HistorialClinico> lista = new List<HistorialClinico>();

            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                // Consulta ajustada a tu estructura de tablas real
                string sql = @"
                                SELECT h.*, 
                                       CONCAT(u.nombre, ' ', u.apellidos) AS nombre_veterinario, 
                                       c.motivo AS motivo_cita
                                FROM historial_clinico h
                                INNER JOIN veterinarios v ON h.id_veterinario = v.id_veterinario
                                INNER JOIN usuarios u ON v.id_usuario = u.id_usuario
                                LEFT JOIN citas c ON h.id_cita = c.id_cita
                                WHERE h.id_mascota = @idMascota
                                ORDER BY h.fecha_hora DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMascota", idMascota);

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HistorialClinico historial = Map(reader);
                            historial.NombreVeterinario = reader["nombre_veterinario"].ToString();
                            historial.Motivo = reader["motivo_cita"].ToString();

                            lista.Add(historial);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener historial clínico: " + ex.Message);
                }
            }
            return lista;
        }

        // =========================
        // ACTUALIZAR
        // =========================

        public bool Actualizar(HistorialClinico h)
        {
            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = @"UPDATE historial_clinico SET
                            peso=@peso,
                            diagnostico=@diagnostico,
                            tratamiento=@tratamiento,
                            observaciones=@observaciones
                            WHERE id_historial=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@peso", h.Peso);
                cmd.Parameters.AddWithValue("@diagnostico", h.Diagnostico);
                cmd.Parameters.AddWithValue("@tratamiento", h.Tratamiento);
                cmd.Parameters.AddWithValue("@observaciones", h.Observaciones);
                cmd.Parameters.AddWithValue("@id", h.IdHistorial);

                conn.Open();

                if (cmd.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // =========================
        // ELIMINAR
        // =========================

        public void Eliminar(int id)
        {
            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = "DELETE FROM historial_clinico WHERE id_historial=@id";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}