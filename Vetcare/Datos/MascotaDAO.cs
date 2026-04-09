using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    class MascotaDAO
    {
        Conexion conexion = new Conexion();

        // ===========================
        // OBTENER MASCOTAS
        // ===========================
        public List<Mascota> ObtenerTodas()
        {
            List<Mascota> lista = new List<Mascota>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                SELECT m.id_mascota,
                       m.id_cliente,
                       m.numero_chip,
                       m.nombre,
                       m.id_raza,
                       r.nombre AS nombre_raza,
                       e.nombre AS nombre_especie,
                       m.sexo,
                       m.peso,
                       m.fecha_nacimiento,
                       m.activo,
                       c.nombre AS nombre_dueno,
                       c.apellidos AS apellidos_dueno,
                       c.num_documento AS documento_dueno
                FROM mascotas m
                INNER JOIN clientes c ON m.id_cliente = c.id_cliente
                INNER JOIN razas r ON m.id_raza = r.id_raza
                INNER JOIN especies e ON r.id_especie = e.id_especie";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(MappingMascota(rdr));
                    }
                }
            }

            return lista;
        }

        public Mascota ObtenerPorId(int idMascota)
        {
            Mascota mascota = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                SELECT m.id_mascota,
                       m.id_cliente,
                       m.numero_chip,
                       m.nombre,
                       m.id_raza,
                       r.nombre AS nombre_raza,
                       e.nombre AS nombre_especie,
                       m.sexo,
                       m.peso,
                       m.fecha_nacimiento,
                       m.activo,
                       c.nombre AS nombre_dueno,
                       c.apellidos AS apellidos_dueno,
                       c.num_documento AS documento_dueno
                FROM mascotas m
                INNER JOIN clientes c ON m.id_cliente = c.id_cliente
                INNER JOIN razas r ON m.id_raza = r.id_raza
                INNER JOIN especies e ON r.id_especie = e.id_especie
                WHERE m.id_mascota = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idMascota);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        mascota = MappingMascota(rdr);
                }
            }

            return mascota;
        }

        public List<Mascota> ObtenerPorCliente(int idCliente)
        {
            List<Mascota> lista = new List<Mascota>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                SELECT m.id_mascota,
                       m.id_cliente,
                       m.numero_chip,
                       m.nombre,
                       m.id_raza,
                       r.nombre AS nombre_raza,
                       e.nombre AS nombre_especie,
                       m.sexo,
                       m.peso,
                       m.fecha_nacimiento,
                       m.activo,
                       c.nombre AS nombre_dueno,
                       c.apellidos AS apellidos_dueno,
                       c.num_documento AS documento_dueno
                FROM mascotas m
                INNER JOIN clientes c ON m.id_cliente = c.id_cliente
                INNER JOIN razas r ON m.id_raza = r.id_raza
                INNER JOIN especies e ON r.id_especie = e.id_especie
                WHERE m.id_cliente = @idCliente";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        lista.Add(MappingMascota(rdr));
                }
            }

            return lista;
        }

        // ===========================
        // INSERTAR / ACTUALIZAR
        // ===========================
        public bool Insertar(Mascota mascota)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"INSERT INTO mascotas
                               (id_cliente, id_raza, numero_chip, nombre, sexo, peso, fecha_nacimiento, activo)
                               VALUES (@idCliente, @idRaza, @numeroChip, @nombre, @sexo, @peso, @fechaNacimiento, TRUE)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, mascota);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Mascota mascota)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"UPDATE mascotas
                               SET id_cliente = @idCliente,
                                   id_raza = @idRaza,
                                   numero_chip = @numeroChip,
                                   nombre = @nombre,
                                   sexo = @sexo,
                                   peso = @peso,
                                   fecha_nacimiento = @fechaNacimiento
                               WHERE id_mascota = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                CargarParametros(cmd, mascota);
                cmd.Parameters.AddWithValue("@id", mascota.IdMascota);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ===========================
        // SOFT DELETE (DESACTIVAR)
        // ===========================
        public bool Desactivar(int idMascota)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "UPDATE mascotas SET activo = FALSE WHERE id_mascota = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idMascota);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DesactivarVarios(List<int> idsMascotas)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string sql = "UPDATE mascotas SET activo = FALSE WHERE id_mascota = @id";

                    foreach (int id in idsMascotas)
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                        cmd.Parameters.AddWithValue("@id", id);
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

        public int ContarMascotas()
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM mascotas WHERE activo = TRUE";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Reactivar(int idMascota)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "UPDATE mascotas SET activo = TRUE WHERE id_mascota = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idMascota);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ===========================
        // MÉTODOS AUXILIARES
        // ===========================
        private void CargarParametros(MySqlCommand cmd, Mascota mascota)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idCliente", mascota.IdCliente);
            cmd.Parameters.AddWithValue("@idRaza", mascota.IdRaza);
            cmd.Parameters.AddWithValue("@numeroChip", mascota.NumeroChip);
            cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
            cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
            cmd.Parameters.AddWithValue("@peso", mascota.Peso);
            cmd.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);
        }

        private Mascota MappingMascota(MySqlDataReader rdr)
        {
            return new Mascota
            {
                IdMascota = Convert.ToInt32(rdr["id_mascota"]),
                IdCliente = Convert.ToInt32(rdr["id_cliente"]),
                IdRaza = Convert.ToInt32(rdr["id_raza"]),
                NumeroChip = rdr["numero_chip"].ToString(),
                Nombre = rdr["nombre"].ToString(),
                Sexo = rdr["sexo"].ToString(),
                Peso = rdr["peso"] != DBNull.Value ? Convert.ToDecimal(rdr["peso"]) : 0,
                FechaNacimiento = rdr["fecha_nacimiento"] != DBNull.Value ? Convert.ToDateTime(rdr["fecha_nacimiento"]) : DateTime.MinValue,
                NombreRaza = rdr["nombre_raza"].ToString(),
                NombreEspecie = rdr["nombre_especie"].ToString(),
                NombreDueno = rdr["nombre_dueno"].ToString(),
                ApellidosDueno = rdr["apellidos_dueno"].ToString(),
                NumeroIdentificacionDueno = rdr["documento_dueno"].ToString(),
                Activo = Convert.ToBoolean(rdr["activo"])
            };
        }
    }
}