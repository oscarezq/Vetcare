using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class FacturaDAO
    {
        private Conexion conexion = new Conexion();

        public bool InsertarFacturaCompleta(Factura f)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                using (MySqlTransaction tra = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Cálculo de totales (Se mantiene igual, solo corregí nombres de propiedad)
                        f.BaseImponible = f.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                        f.IvaTotal = f.Detalles.Sum(d => (d.Cantidad * d.PrecioUnitario) * (d.IvaPorcentaje / 100));
                        f.Total = f.BaseImponible + f.IvaTotal;

                        // 2. Insertar Cabecera
                        string sqlFactura = @"INSERT INTO facturas 
                            (id_cliente, numero_factura, estado, base_imponible, iva_total, total, metodo_pago, fecha_emision, observaciones) 
                            VALUES (@cli, @num, @est, @base, @iva, @total, @met, @fec, @obs)"; // Añadido @obs

                        int idFacturaGenerada;
                        using (MySqlCommand cmdF = new MySqlCommand(sqlFactura, con, tra))
                        {
                            cmdF.Parameters.AddWithValue("@cli", f.IdCliente);
                            cmdF.Parameters.AddWithValue("@num", f.NumeroFactura);
                            cmdF.Parameters.AddWithValue("@est", f.Estado ?? "Pendiente");
                            cmdF.Parameters.AddWithValue("@base", f.BaseImponible);
                            cmdF.Parameters.AddWithValue("@iva", f.IvaTotal);
                            cmdF.Parameters.AddWithValue("@total", f.Total);
                            cmdF.Parameters.AddWithValue("@met", f.MetodoPago);
                            cmdF.Parameters.AddWithValue("@fec", DateTime.Now);
                            cmdF.Parameters.AddWithValue("@obs", (object)f.Observaciones ?? DBNull.Value); // Asignación de @obs

                            cmdF.ExecuteNonQuery();
                            idFacturaGenerada = Convert.ToInt32(cmdF.LastInsertedId);
                        }

                        // 3. Insertar Detalles (Corregido según tu tabla SQL)
                        string sqlDetalle = @"INSERT INTO detalles_factura 
                    (id_factura, id_concepto, nombre_concepto, tipo, cantidad, precio_unitario, iva_porcentaje, subtotal, iva_importe, total_linea) 
                    VALUES (@idF, @idC, @nom, @tipo, @cant, @pre, @ivaP, @sub, @ivaI, @totL)";

                        using (MySqlCommand cmdD = new MySqlCommand(sqlDetalle, con, tra))
                        {
                            // Preparamos los parámetros una sola vez por rendimiento
                            cmdD.Parameters.Add("@idF", MySqlDbType.Int32);
                            cmdD.Parameters.Add("@idC", MySqlDbType.Int32);
                            cmdD.Parameters.Add("@nom", MySqlDbType.VarChar);
                            cmdD.Parameters.Add("@tipo", MySqlDbType.VarChar);
                            cmdD.Parameters.Add("@cant", MySqlDbType.Int32);
                            cmdD.Parameters.Add("@pre", MySqlDbType.Decimal);
                            cmdD.Parameters.Add("@ivaP", MySqlDbType.Decimal);
                            cmdD.Parameters.Add("@sub", MySqlDbType.Decimal);
                            cmdD.Parameters.Add("@ivaI", MySqlDbType.Decimal);
                            cmdD.Parameters.Add("@totL", MySqlDbType.Decimal);

                            foreach (var det in f.Detalles)
                            {
                                decimal subtotal = det.Cantidad * det.PrecioUnitario;
                                decimal ivaImporte = subtotal * (det.IvaPorcentaje / 100);

                                cmdD.Parameters["@idF"].Value = idFacturaGenerada;
                                cmdD.Parameters["@idC"].Value = det.IdConcepto;
                                cmdD.Parameters["@nom"].Value = det.NombreConcepto;
                                cmdD.Parameters["@tipo"].Value = det.Tipo;
                                cmdD.Parameters["@cant"].Value = det.Cantidad;
                                cmdD.Parameters["@pre"].Value = det.PrecioUnitario;
                                cmdD.Parameters["@ivaP"].Value = det.IvaPorcentaje;
                                cmdD.Parameters["@sub"].Value = subtotal;
                                cmdD.Parameters["@ivaI"].Value = ivaImporte;
                                cmdD.Parameters["@totL"].Value = subtotal + ivaImporte;

                                cmdD.ExecuteNonQuery();

                                // 4. EXTRA: Descontar stock si es Producto
                                if (det.Tipo == "Producto")
                                {
                                    string sqlStock = "UPDATE conceptos SET stock = stock - @c WHERE id_concepto = @id";
                                    using (MySqlCommand cmdS = new MySqlCommand(sqlStock, con, tra))
                                    {
                                        cmdS.Parameters.AddWithValue("@c", det.Cantidad);
                                        cmdS.Parameters.AddWithValue("@id", det.IdConcepto);
                                        cmdS.ExecuteNonQuery();
                                    }
                                }
                            }
                        }

                        tra.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        throw new Exception("Error al procesar la factura: " + ex.Message);
                    }
                }
            }
        }

        public List<Factura> ObtenerTodas()
        {
            List<Factura> lista = new List<Factura>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // JOIN para obtener el nombre del cliente desde la tabla usuarios
                string sql = @"SELECT f.*, c.nombre as nombre_cliente 
                               FROM facturas f 
                               INNER JOIN clientes c ON f.id_cliente = c.id_cliente
                               ORDER BY f.fecha_emision DESC";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Factura f = MapearFactura(dr);
                        // Asignamos el nombre que viene del JOIN
                        f.NombreCliente = dr["nombre_cliente"].ToString();
                        lista.Add(f);
                    }
                }
            }
            return lista;
        }

        public bool AnularFactura(int idFactura)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Actualizamos el estado y añadimos una nota en observaciones
                string sql = @"UPDATE facturas 
                               SET estado = 'Anulada', 
                                   observaciones = CONCAT(IFNULL(observaciones,''), ' [ANULADA ', NOW(), ']') 
                               WHERE id_factura = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idFactura);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public string ObtenerUltimoNumeroPorAnio(int anioActual)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Usamos CONCAT para MySQL
                string sql = @"SELECT MAX(numero_factura) 
                       FROM Facturas 
                       WHERE numero_factura LIKE CONCAT(@anio, '-%')";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@anio", anioActual.ToString());

                    con.Open();
                    object result = cmd.ExecuteScalar();

                    // Si es nulo o vacío, devolvemos null para facilitar la lógica posterior
                    if (result == DBNull.Value || result == null)
                        return null;

                    return result.ToString();
                }
            }
        }

        public bool ActualizarEstadoFactura(int idFactura, string nuevoEstado)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"UPDATE facturas 
                       SET estado = @estado
                       WHERE id_factura = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", idFactura);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Factura MapearFactura(MySqlDataReader dr)
        {
            return new Factura
            {
                IdFactura = Convert.ToInt32(dr["id_factura"]),
                IdCliente = Convert.ToInt32(dr["id_cliente"]),
                NumeroFactura = dr["numero_factura"].ToString(),
                Estado = dr["estado"].ToString(),
                FechaEmision = Convert.ToDateTime(dr["fecha_emision"]),
                BaseImponible = dr["base_imponible"] != DBNull.Value ? Convert.ToDecimal(dr["base_imponible"]) : 0,
                IvaTotal = dr["iva_total"] != DBNull.Value ? Convert.ToDecimal(dr["iva_total"]) : 0,
                Total = Convert.ToDecimal(dr["total"]),
                MetodoPago = dr["metodo_pago"].ToString(),
                Observaciones = dr["observaciones"].ToString()
            };
        }
    }
}