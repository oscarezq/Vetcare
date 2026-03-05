using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    class RazaDAO
    {
        Conexion conexion = new Conexion();

        public List<Raza> ObtenerTodas()
        {
            List<Raza> lista = new List<Raza>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT 
                        r.id_raza,
                        r.nombre,
                        r.id_especie,
                        e.nombre AS nombre_especie
                    FROM razas r
                    INNER JOIN especies e ON r.id_especie = e.id_especie";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(MappingRaza(rdr));
                    }
                }
            }

            return lista;
        }

        public Raza ObtenerPorId(int id)
        {
            Raza raza = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT 
                        r.id_raza,
                        r.nombre,
                        r.id_especie,
                        e.nombre AS nombre_especie
                    FROM razas r
                    INNER JOIN especies e ON r.id_especie = e.id_especie
                    WHERE r.id_raza = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        raza = MappingRaza(rdr);
                }
            }

            return raza;
        }

        public bool Insertar(Raza raza)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "INSERT INTO razas (nombre, id_especie) VALUES (@nombre, @idEspecie)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombre", raza.NombreRaza);
                cmd.Parameters.AddWithValue("@idEspecie", raza.IdEspecie);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Raza raza)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    UPDATE razas 
                    SET nombre = @nombre, id_especie = @idEspecie
                    WHERE id_raza = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombre", raza.NombreRaza);
                cmd.Parameters.AddWithValue("@idEspecie", raza.IdEspecie);
                cmd.Parameters.AddWithValue("@id", raza.IdRaza);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int id)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "DELETE FROM razas WHERE id_raza = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Raza MappingRaza(MySqlDataReader rdr)
        {
            return new Raza
            {
                IdRaza = Convert.ToInt32(rdr["id_raza"]),
                NombreRaza = rdr["nombre"].ToString(),
                IdEspecie = Convert.ToInt32(rdr["id_especie"]),
                NombreEspecie = rdr["nombre_especie"].ToString()
            };
        }
    }
}