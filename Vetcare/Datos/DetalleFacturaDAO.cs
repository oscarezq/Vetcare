using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad DetalleFactura.
    /// Gestiona las operaciones de consulta de los detalles asociados a una factura
    /// en la base de datos.
    /// </summary>
    public class DetalleFacturaDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todos los detalles asociados a una factura específica.
        /// </summary>
        /// <param name="idFactura">Identificador de la factura.</param>
        /// <returns>Lista de detalles de la factura.</returns>
        public List<DetalleFactura> ObtenerDetallesPorFactura(int idFactura)
        {
            List<DetalleFactura> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"SELECT * 
                               FROM detalles_factura 
                               WHERE id_factura = @idF";

                MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@idF", idFactura);

                try
                {
                    con.Open();

                    using MySqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(MappingDetalleFactura(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener los detalles: " + ex.Message);
                }
            }

            return lista;
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto DetalleFactura.
        /// </summary>
        /// <param name="dr">Lector de datos.</param>
        /// <returns>Objeto DetalleFactura.</returns>
        private static DetalleFactura MappingDetalleFactura(MySqlDataReader dr)
        {
            return new DetalleFactura
            {
                IdDetalle = Convert.ToInt32(dr["id_detalle"]),
                IdFactura = Convert.ToInt32(dr["id_factura"]),
                IdConcepto = dr["id_concepto"] == DBNull.Value ? 0 : Convert.ToInt32(dr["id_concepto"]),
                NombreConcepto = dr["nombre_concepto"].ToString(),
                Tipo = dr["tipo"].ToString(),
                Cantidad = Convert.ToInt32(dr["cantidad"]),
                PrecioUnitario = Convert.ToDecimal(dr["precio_unitario"]),
                IvaPorcentaje = Convert.ToDecimal(dr["iva_porcentaje"])
            };
        }
    }
}