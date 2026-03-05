using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    class EspecieDAO
    {
        Conexion conexion = new Conexion();

        public List<Especie> ObtenerTodas()
        {
            List<Especie> lista = new List<Especie>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "SELECT id_especie, nombre FROM especies";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(MappingEspecie(rdr));
                    }
                }
            }

            return lista;
        }

        public Especie ObtenerPorId(int id)
        {
            Especie especie = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "SELECT id_especie, nombre FROM especies WHERE id_especie = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        especie = MappingEspecie(rdr);
                }
            }

            return especie;
        }

        public bool Insertar(Especie especie)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "INSERT INTO especies (nombre) VALUES (@nombre)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombre", especie.NombreEspecie);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Especie especie)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "UPDATE especies SET nombre = @nombre WHERE id_especie = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nombre", especie.NombreEspecie);
                cmd.Parameters.AddWithValue("@id", especie.IdEspecie);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Eliminar(int id)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "DELETE FROM especies WHERE id_especie = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Especie MappingEspecie(MySqlDataReader rdr)
        {
            return new Especie
            {
                IdEspecie = Convert.ToInt32(rdr["id_especie"]),
                NombreEspecie = rdr["nombre"].ToString()
            };
        }
    }
}