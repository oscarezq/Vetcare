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
                IdHistorial = reader.GetInt32("id_historial"),
                IdMascota = reader.GetInt32("id_mascota"),
                IdCita = reader.IsDBNull(reader.GetOrdinal("id_cita")) ? null : reader.GetInt32("id_cita"),
                IdVeterinario = reader.GetInt32("id_veterinario"),
                FechaHora = reader.GetDateTime("fecha_hora"),
                Peso = reader.IsDBNull(reader.GetOrdinal("peso")) ? null : reader.GetDecimal("peso"),
                Diagnostico = reader.GetString("diagnostico"),
                Tratamiento = reader.IsDBNull(reader.GetOrdinal("tratamiento")) ? null : reader.GetString("tratamiento"),
                Observaciones = reader.IsDBNull(reader.GetOrdinal("observaciones")) ? null : reader.GetString("observaciones")
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

        public List<HistorialClinico> ListarPorMascota(int idMascota)
        {
            List<HistorialClinico> lista = new List<HistorialClinico>();

            using (MySqlConnection conn = conexion.ObtenerConexion())
            {
                string sql = @"SELECT * 
                               FROM historial_clinico 
                               WHERE id_mascota=@idMascota
                               ORDER BY fecha_hora DESC";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@idMascota", idMascota);

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