using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Especie.
    /// Gestiona las operaciones de consulta e inserción de especies en la base de datos.
    /// </summary>
    class EspecieDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todas las especies registradas en el sistema.
        /// </summary>
        /// <returns>Lista de especies.</returns>
        public List<Especie> ObtenerTodas()
        {
            List<Especie> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT id_especie,
                                  nombre
                           FROM especies";

            MySqlCommand cmd = new(sql, con);

            using MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                lista.Add(MapearEspecie(rdr));
            }

            return lista;
        }

        /// <summary>
        /// Inserta una nueva especie en la base de datos.
        /// </summary>
        /// <param name="especie">Objeto especie a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(Especie especie)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"INSERT INTO especies (nombre) 
                           VALUES (@nombre)";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@nombre", especie.NombreEspecie);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Especie.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Especie.</returns>
        private static Especie MapearEspecie(MySqlDataReader rdr)
        {
            return new Especie
            {
                IdEspecie = Convert.ToInt32(rdr["id_especie"]),
                NombreEspecie = rdr["nombre"].ToString()
            };
        }
    }
}