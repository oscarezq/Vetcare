using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    class DetalleFacturaDAO
    {
        private Conexion conexion = new Conexion();

        public List<DetalleFactura> ObtenerDetallesPorFactura(int idFactura)
        {
            List<DetalleFactura> lista = new List<DetalleFactura>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"SELECT d.*, s.nombre as nombre_servicio 
                       FROM detalles_factura d 
                       INNER JOIN servicios s ON d.id_servicio = s.id_servicio 
                       WHERE d.id_factura = @idF";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idF", idFactura);
                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new DetalleFactura
                        {
                            IdDetalle = Convert.ToInt32(dr["id_detalle"]),
                            IdFactura = Convert.ToInt32(dr["id_factura"]),
                            IdServicio = Convert.ToInt32(dr["id_servicio"]),
                            NombreServicio = dr["nombre_servicio"].ToString(),
                            Cantidad = Convert.ToInt32(dr["cantidad"]),
                            PrecioUnitario = Convert.ToDecimal(dr["precio_unitario"])
                        });
                    }
                }
            }
            return lista;
        }
    }
}
