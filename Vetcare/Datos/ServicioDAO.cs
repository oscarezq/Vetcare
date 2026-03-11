using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class ServicioDAO
    {
        private Conexion conexion = new Conexion();

        public List<Servicio> ObtenerTodos()
        {
            List<Servicio> lista = new List<Servicio>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM servicios ORDER BY nombre ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read()) lista.Add(MapearServicio(dr));
                    }
                }
                catch (Exception ex) { throw new Exception("Error al obtener servicios: " + ex.Message); }
            }
            return lista;
        }

        public bool Insertar(Servicio s)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "INSERT INTO servicios (nombre, descripcion, precio_base) VALUES (@nom, @desc, @pre)";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@nom", s.Nombre);
                cmd.Parameters.AddWithValue("@desc", s.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pre", s.PrecioBase);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Actualizar(Servicio s)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // SQL con la cláusula WHERE para afectar solo al registro específico
                string sql = @"UPDATE servicios 
                       SET nombre = @nom, 
                           descripcion = @desc, 
                           precio_base = @pre 
                       WHERE id_servicio = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                // Pasamos los parámetros
                cmd.Parameters.AddWithValue("@id", s.IdServicio);
                cmd.Parameters.AddWithValue("@nom", s.Nombre);
                cmd.Parameters.AddWithValue("@desc", s.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@pre", s.PrecioBase);

                try
                {
                    con.Open();
                    // Retorna true si se modificó al menos una fila
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar el servicio: " + ex.Message);
                }
            }
        }

        private Servicio MapearServicio(MySqlDataReader dr)
        {
            return new Servicio
            {
                IdServicio = Convert.ToInt32(dr["id_servicio"]),
                Nombre = dr["nombre"].ToString(),
                Descripcion = dr["descripcion"].ToString(),
                PrecioBase = Convert.ToDecimal(dr["precio_base"])
            };
        }
    }
}
