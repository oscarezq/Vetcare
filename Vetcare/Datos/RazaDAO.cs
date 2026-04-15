using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Raza.
    /// Gestiona las operaciones de consulta, inserción, actualización y eliminación
    /// de razas en la base de datos.
    /// </summary>
    class RazaDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todas las razas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de razas.</returns>
        public List<Raza> ObtenerTodas()
        {
            List<Raza> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT r.id_raza,
                           r.nombre,
                           r.id_especie,
                           e.nombre AS nombre_especie
                    FROM razas r
                        INNER JOIN especies e ON r.id_especie = e.id_especie";

                MySqlCommand cmd = new(sql, con);

                using MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lista.Add(MappingRaza(rdr));
                }
            }

            return lista;
        }

        /// <summary>
        /// Obtiene todas las razas asociadas a una especie específica.
        /// </summary>
        /// <param name="idEspecie">Identificador de la especie.</param>
        /// <returns>Lista de razas de la especie.</returns>
        public List<Raza> ObtenerPorEspecie(int idEspecie)
        {
            List<Raza> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT r.id_raza,
                        r.nombre,
                        r.id_especie,
                        e.nombre AS nombre_especie
                    FROM razas r
                        INNER JOIN especies e ON r.id_especie = e.id_especie
                    WHERE r.id_especie = @idEspecie";

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idEspecie", idEspecie);

                using MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lista.Add(MappingRaza(rdr));
                }
            }

            return lista;
        }

        /// <summary>
        /// Inserta una nueva raza en la base de datos.
        /// </summary>
        /// <param name="raza">Objeto raza a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(Raza raza)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"INSERT INTO razas (nombre, id_especie) 
                           VALUES (@nombre, @idEspecie)";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@nombre", raza.NombreRaza);
            cmd.Parameters.AddWithValue("@idEspecie", raza.IdEspecie);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Raza.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Raza.</returns>
        private static Raza MappingRaza(MySqlDataReader rdr)
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