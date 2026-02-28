using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
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

                string sql = @"SELECT c.id_cita, c.id_mascota, c.id_veterinario, c.fecha, 
                       c.motivo, c.estado, c.observaciones,
                       m.nombre AS nombre_mascota,
                       CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno,
                       CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario,
                       v.numero_colegiado
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
                string sql = @"SELECT c.id_cita, c.id_mascota, c.id_veterinario, c.fecha, 
                       c.motivo, c.estado, c.observaciones,
                       m.nombre AS nombre_mascota,
                       CONCAT(cli.nombre,' ',cli.apellidos) AS nombre_dueno,
                       CONCAT(uv.nombre,' ',uv.apellidos) AS nombre_veterinario,
                       v.numero_colegiado
                FROM citas c
                INNER JOIN mascotas m ON c.id_mascota = m.id_mascota
                INNER JOIN clientes cli ON m.id_cliente = cli.id_cliente
                INNER JOIN veterinarios v ON c.id_veterinario = v.id_veterinario
                INNER JOIN usuarios uv ON v.id_usuario = uv.id_usuario";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idCita);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read()) cita = MappingCita(rdr);
                }
            }
            return cita;
        }

        public bool Insertar(Cita cita)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                // CORREGIDO: fecha_hora -> fecha
                string sql = @"INSERT INTO citas (id_mascota, id_veterinario, fecha, motivo, estado, observaciones)
                               VALUES (@idMascota, @idVeterinario, @fecha, @motivo, @estado, @observaciones)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, cita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool InsertarVarias(List<Cita> citas)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        string sql = @"INSERT INTO citas (id_mascota, id_veterinario, fecha, motivo, estado, observaciones)
                                       VALUES (@idMascota, @idVeterinario, @fecha, @motivo, @estado, @observaciones)";

                        foreach (Cita cita in citas)
                        {
                            MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                            CargarParametros(cmd, cita);
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

        public bool Actualizar(Cita cita)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"UPDATE citas SET id_mascota = @idMascota, id_veterinario = @idVeterinario,
                                                fecha = @fecha, motivo = @motivo, estado = @estado, 
                                                observaciones = @observaciones
                               WHERE id_cita = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, cita);
                cmd.Parameters.AddWithValue("@id", cita.IdCita);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarVarias(List<Cita> citas)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction transaccion = con.BeginTransaction())
                {
                    try
                    {
                        string sql = @"UPDATE citas SET id_mascota = @idMascota, id_veterinario = @idVeterinario,
                                                        fecha = @fecha, motivo = @motivo, estado = @estado, 
                                                        observaciones = @observaciones
                                       WHERE id_cita = @id";

                        foreach (Cita cita in citas)
                        {
                            MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                            CargarParametros(cmd, cita);
                            cmd.Parameters.AddWithValue("@id", cita.IdCita);
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

                        // Creamos el comando una sola vez fuera del bucle por rendimiento
                        MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                        cmd.Parameters.Add("@id", MySqlDbType.Int32);

                        foreach (int id in idsCitas)
                        {
                            // Asignamos el valor al parámetro existente
                            cmd.Parameters["@id"].Value = id;
                            cmd.ExecuteNonQuery();
                        }

                        // Si todo fue bien, confirmamos los cambios
                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        // Si hubo un error (ej. cita inexistente o bloqueada), deshacemos todo
                        transaccion.Rollback();
                        return false;
                    }
                }
            }
        }

        // METODOS AUXILIARES PARA EVITAR REPETIR CÓDIGO
        private void CargarParametros(MySqlCommand cmd, Cita cita)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idMascota", cita.IdMascota);
            cmd.Parameters.AddWithValue("@idVeterinario", cita.IdVeterinario);
            cmd.Parameters.AddWithValue("@fecha", cita.FechaHora);
            cmd.Parameters.AddWithValue("@motivo", cita.Motivo);
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
                FechaHora = Convert.ToDateTime(rdr["fecha"]),
                Motivo = rdr["motivo"].ToString(),
                Estado = rdr["estado"].ToString(),
                Observaciones = rdr["observaciones"].ToString(),
                NombreMascota = rdr["nombre_mascota"].ToString(),
                NombreDueno = rdr["nombre_dueno"].ToString(),
                NombreVeterinario = rdr["nombre_veterinario"].ToString(),
                NumeroColegiado = rdr["numero_colegiado"].ToString()
            };
        }
    }
}