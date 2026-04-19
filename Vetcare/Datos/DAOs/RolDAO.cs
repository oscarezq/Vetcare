using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos.DAOs
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Rol.
    /// Gestiona las operaciones de consulta de roles en la base de datos.
    /// </summary>
    public class RolDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene la lista de todos los roles disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de roles ordenados alfabéticamente.</returns>
        public List<Rol> Listar()
        {
            List<Rol> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT id_rol, nombre " +
                             "FROM roles " +
                             "ORDER BY nombre ASC";
                MySqlCommand cmd = new(sql, con);

                try
                {
                    con.Open();

                    using MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Rol
                        {
                            IdRol = Convert.ToInt32(dr["id_rol"]),
                            NombreRol = dr["nombre"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al listar roles: " + ex.Message);
                }
            }

            return lista;
        }
    }
}