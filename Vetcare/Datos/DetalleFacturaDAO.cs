using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class DetalleFacturaDAO // Cambiado a public para poder usarlo desde otros proyectos
    {
        private Conexion conexion = new Conexion();

        public List<DetalleFactura> ObtenerDetallesPorFactura(int idFactura)
        {
            List<DetalleFactura> lista = new List<DetalleFactura>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Ya no necesitamos INNER JOIN porque la tabla detalles_factura 
                // ya tiene nombre_concepto, tipo e iva_porcentaje guardados.
                string sql = @"SELECT * FROM detalles_factura WHERE id_factura = @idF";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idF", idFactura);

                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new DetalleFactura
                            {
                                IdDetalle = Convert.ToInt32(dr["id_detalle"]),
                                IdFactura = Convert.ToInt32(dr["id_factura"]),
                                IdConcepto = dr["id_concepto"] == DBNull.Value ? 0 : Convert.ToInt32(dr["id_concepto"]),
                                NombreConcepto = dr["nombre_concepto"].ToString(),
                                Tipo = dr["tipo"].ToString(),
                                Cantidad = Convert.ToInt32(dr["cantidad"]),
                                PrecioUnitario = Convert.ToDecimal(dr["precio_unitario"]),
                                IvaPorcentaje = Convert.ToDecimal(dr["iva_porcentaje"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los detalles: " + ex.Message);
                }
            }
            return lista;
        }
    }
}