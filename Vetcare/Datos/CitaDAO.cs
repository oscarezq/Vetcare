using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    class CitaDAO
    {
        Conexion conexion = new Conexion();

        public List<Cita> ObtenerTodas()
        {
            List<Cita> lista = new List<Cita>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT 
                        c.id_cita, 
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
                    FROM citas c
                    INNER JOIN mascotas m ON c.id_mascota = m.id_mascota
                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente
                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario
                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(MappingCita(rdr));
                    }
                }
            }
            return lista;
        }

        public Cita ObtenerPorId(int idCita)
        {
            Cita cita = null;
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"
                    SELECT 
                        c.id_cita, 
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
                    FROM citas c
                    INNER JOIN mascotas m ON c.id_mascota = m.id_mascota
                    INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente
                    INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario
                    INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario
                    WHERE c.id_cita = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idCita);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        cita = MappingCita(rdr);
                }
            }
            return cita;
        }

        public bool Insertar(Cita cita)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"
            INSERT INTO citas 
                (id_mascota, id_veterinario, fecha_hora, duracion_estimada, motivo, estado, observaciones)
            VALUES 
                (@idMascota, @idVeterinario, @fecha_hora, @duracion_estimada, @motivo, @estado, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, cita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Cita cita)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"
            UPDATE citas SET 
                id_mascota = @idMascota, 
                id_veterinario = @idVeterinario,
                fecha_hora = @fecha_hora,
                duracion_estimada = @duracion_estimada,
                motivo = @motivo, 
                estado = @estado, 
                observaciones = @observaciones
            WHERE id_cita = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, cita);
                cmd.Parameters.AddWithValue("@id", cita.IdCita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarEstado(int idCita, string estado)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @" UPDATE citas 
                    SET estado = @estado
                    WHERE id_cita = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@id", idCita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int idCita)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = "DELETE FROM citas WHERE id_cita = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idCita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool EliminarVarias(List<int> idsCitas)
        {
            if (idsCitas == null || idsCitas.Count == 0) return false;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        string sql = "DELETE FROM citas WHERE id_cita = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                        cmd.Parameters.Add("@id", MySqlDbType.Int32);

                        foreach (int id in idsCitas)
                        {
                            cmd.Parameters["@id"].Value = id;
                            cmd.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        return false;
                    }
                }
            }
        }

        // KPI citas hoy
        public int ContarCitasHoy()
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM citas WHERE DATE(fecha_hora) = CURDATE()";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Próximas citas
        public List<Cita> ObtenerProximasCitas()
        {
            List<Cita> lista = new List<Cita>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                SELECT c.id_cita,
                       c.fecha_hora,
                       c.motivo,
                       m.nombre AS mascota,
                       CONCAT(cl.nombre, ' ', cl.apellidos) AS cliente
                FROM citas c
                INNER JOIN mascotas m ON c.id_mascota = m.id_mascota
                INNER JOIN clientes cl ON m.id_cliente = cl.id_cliente
                WHERE c.fecha_hora >= NOW()
                ORDER BY c.fecha_hora
                LIMIT 10";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new Cita
                        {
                            IdCita = Convert.ToInt32(rdr["id_cita"]),
                            FechaHora = Convert.ToDateTime(rdr["fecha_hora"]),
                            Motivo = rdr["motivo"].ToString(),
                            NombreMascota = rdr["mascota"].ToString(),
                            NombreDueno = rdr["cliente"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        // --- MÉTODOS AUXILIARES ---
        private void CargarParametros(MySqlCommand cmd, Cita cita)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idMascota", cita.IdMascota);
            cmd.Parameters.AddWithValue("@idVeterinario", cita.IdVeterinario);
            cmd.Parameters.AddWithValue("@fecha_hora", cita.FechaHora);
            cmd.Parameters.AddWithValue("@duracion_estimada", cita.DuracionEstimada);
            cmd.Parameters.AddWithValue("@motivo", (object)cita.Motivo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@estado", cita.Estado);
            cmd.Parameters.AddWithValue("@observaciones", cita.Observaciones);
        }

        private Cita MappingCita(MySqlDataReader rdr)
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