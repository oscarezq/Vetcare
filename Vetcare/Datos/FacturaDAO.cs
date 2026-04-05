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
                        // 1. Cálculo de totales
                        f.BaseImponible = f.Detalles.Sum(d => d.Subtotal);
                        f.IvaTotal = f.Detalles.Sum(d => d.IvaImporte);
                        f.Total = f.Detalles.Sum(d => d.TotalLinea);

                        // 2. Insertar Cabecera (Siguiendo tu esquema SQL exacto)
                        string sqlFactura = @"INSERT INTO facturas 
                            (id_cliente, numero_factura, estado, base_imponible, iva_total, total, metodo_pago, fecha_emision, observaciones) 
                            VALUES (@cli, @num, @est, @base, @iva, @total, @met, @fec, @obs)";

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
                            cmdF.Parameters.AddWithValue("@obs", (object)f.Observaciones ?? DBNull.Value);

                            cmdF.ExecuteNonQuery();
                            idFacturaGenerada = Convert.ToInt32(cmdF.LastInsertedId);
                        }

                        // 3. Insertar Detalles
                        string sqlDetalle = @"INSERT INTO detalles_factura 
                            (id_factura, id_concepto, nombre_concepto, tipo, cantidad, precio_unitario, iva_porcentaje, subtotal, iva_importe, total_linea) 
                            VALUES (@idF, @idC, @nom, @tipo, @cant, @pre, @ivaP, @sub, @ivaI, @totL)";

                        using (MySqlCommand cmdD = new MySqlCommand(sqlDetalle, con, tra))
                        {
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
                                decimal subtotal = det.Subtotal;
                                decimal ivaImporte = det.IvaImporte;
                                decimal totalLinea = det.TotalLinea;

                                cmdD.Parameters["@idF"].Value = idFacturaGenerada;
                                cmdD.Parameters["@idC"].Value = (object)det.IdConcepto ?? DBNull.Value;
                                cmdD.Parameters["@nom"].Value = det.NombreConcepto;
                                cmdD.Parameters["@tipo"].Value = det.Tipo;
                                cmdD.Parameters["@cant"].Value = det.Cantidad;
                                cmdD.Parameters["@pre"].Value = det.PrecioUnitario;
                                cmdD.Parameters["@ivaP"].Value = det.IvaPorcentaje;
                                cmdD.Parameters["@sub"].Value = subtotal;
                                cmdD.Parameters["@ivaI"].Value = ivaImporte;
                                cmdD.Parameters["@totL"].Value = totalLinea;

                                cmdD.ExecuteNonQuery();

                                // 4. Descontar stock si es Producto (Tabla conceptos)
                                if (det.Tipo == "Producto" && det.IdConcepto > 0)
                                {
                                    string sqlStock = "UPDATE conceptos SET stock = stock - @c WHERE id_concepto = @id AND stock IS NOT NULL";
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
                // JOIN con la tabla clientes para obtener nombre, apellidos y documento
                string sql = @"SELECT f.*, 
                               c.nombre AS nombre_cliente, 
                               c.apellidos AS apellidos_cliente, 
                               c.num_documento AS documento_cliente
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
                        // Datos extendidos del cliente para la UI
                        f.NombreCliente = dr["nombre_cliente"].ToString();
                        f.ApellidosCliente = dr["apellidos_cliente"].ToString();
                        f.NumeroDocumentoCliente = dr["documento_cliente"].ToString();
                        lista.Add(f);
                    }
                }
            }
            return lista;
        }

        public List<Factura> ObtenerPorCliente(int idCliente)
        {
            List<Factura> lista = new List<Factura>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // JOIN con la tabla clientes para obtener nombre, apellidos y documento
                string sql = @"SELECT f.*, 
                               c.nombre AS nombre_cliente, 
                               c.apellidos AS apellidos_cliente, 
                               c.num_documento AS documento_cliente
                        FROM facturas f 
                        INNER JOIN clientes c ON f.id_cliente = c.id_cliente
                        WHERE f.id_cliente = @idCliente
                        ORDER BY f.fecha_emision DESC";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);
                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Factura f = MapearFactura(dr);
                        // Datos extendidos del cliente para la UI
                        f.NombreCliente = dr["nombre_cliente"].ToString();
                        f.ApellidosCliente = dr["apellidos_cliente"].ToString();
                        f.NumeroDocumentoCliente = dr["documento_cliente"].ToString();
                        lista.Add(f);
                    }
                }
            }
            return lista;
        }

        public List<Factura> ObtenerFacturasPendientes()
        {
            List<Factura> lista = new List<Factura>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Traemos las facturas pendientes con el nombre del cliente para el Dashboard
                string sql = @"SELECT f.*, 
                               c.nombre AS nombre_cliente, 
                               c.apellidos AS apellidos_cliente
                        FROM facturas f 
                        INNER JOIN clientes c ON f.id_cliente = c.id_cliente
                        WHERE f.estado = 'Pendiente'
                        ORDER BY f.fecha_emision ASC"; // ASC para ver las más antiguas primero

                MySqlCommand cmd = new MySqlCommand(sql, con);

                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            // Usamos tu mapeador existente
                            Factura f = MapearFactura(dr);

                            // Asignamos los nombres para que se vean en el DataGrid
                            f.NombreCliente = dr["nombre_cliente"].ToString();
                            f.ApellidosCliente = dr["apellidos_cliente"].ToString();

                            lista.Add(f);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener facturas pendientes: " + ex.Message);
                }
            }
            return lista;
        }

        public decimal CalcularDeudaCliente(int idCliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Sumamos el total de las facturas que están en estado 'Pendiente'
                string sql = @"SELECT IFNULL(SUM(total), 0) 
                       FROM facturas 
                       WHERE id_cliente = @idCliente 
                       AND estado = 'Pendiente'";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);

                con.Open();
                object result = cmd.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
        }

        public bool AnularFactura(int idFactura)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"UPDATE facturas 
                               SET estado = 'Anulada', 
                                   observaciones = CONCAT(IFNULL(observaciones,''), ' [ANULADA EL ', NOW(), ']') 
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
                string sql = @"SELECT MAX(numero_factura) 
                               FROM facturas 
                               WHERE numero_factura LIKE CONCAT(@anio, '-%')";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@anio", anioActual.ToString());
                    con.Open();
                    object result = cmd.ExecuteScalar();

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
                string sql = @"UPDATE facturas SET estado = @estado WHERE id_factura = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", idFactura);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public decimal ObtenerIngresosMes()
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // Modificamos el SQL para filtrar por el mes y año actuales
                string sql = @"SELECT IFNULL(SUM(total), 0) 
                       FROM facturas 
                       WHERE MONTH(fecha_emision) = MONTH(CURDATE()) 
                       AND YEAR(fecha_emision) = YEAR(CURDATE())
                       AND estado = 'Pagada'";

                MySqlCommand cmd = new MySqlCommand(sql, con);

                object result = cmd.ExecuteScalar();

                return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
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
                Observaciones = dr["observaciones"] != DBNull.Value ? dr["observaciones"].ToString() : ""
            };
        }
    }
}