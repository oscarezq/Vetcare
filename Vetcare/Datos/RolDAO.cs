using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class RolDAO
    {
        private Conexion conexion = new Conexion();

        public List<Rol> Listar()
        {
            List<Rol> lista = new List<Rol>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT id_rol, nombre FROM roles ORDER BY nombre ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Rol
                            {
                                IdRol = Convert.ToInt32(dr["id_rol"]),
                                NombreRol = dr["nombre"].ToString()
                            });
                        }
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