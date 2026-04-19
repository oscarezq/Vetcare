using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using Vetcare.Entidades;

namespace Vetcare.Datos.DAOs
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Factura.
    /// Gestiona las operaciones de creación, consulta, actualización y anulación
    /// de facturas en la base de datos.
    /// </summary>
    public class FacturaDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todas las facturas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de facturas.</returns>
        public List<Factura> ObtenerTodas()
        {
            List<Factura> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT f.*, 
                                  c.nombre AS nombre_cliente, 
                                  c.apellidos AS apellidos_cliente, 
                                  c.num_documento AS documento_cliente
                           FROM facturas f INNER JOIN clientes c 
                               ON f.id_cliente = c.id_cliente
                           ORDER BY f.fecha_emision DESC";

            MySqlCommand cmd = new(sql, con);

            using MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Factura f = MappingFactura(dr);
                f.NombreCliente = dr["nombre_cliente"].ToString();
                f.ApellidosCliente = dr["apellidos_cliente"].ToString();
                f.NumeroDocumentoCliente = dr["documento_cliente"].ToString();
                lista.Add(f);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene todas las facturas de un cliente específico.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <returns>Lista de facturas del cliente.</returns>
        public List<Factura> ObtenerPorCliente(int idCliente)
        {
            List<Factura> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT f.*, 
                                  c.nombre AS nombre_cliente, 
                                  c.apellidos AS apellidos_cliente, 
                                  c.num_documento AS documento_cliente
                           FROM facturas f INNER JOIN clientes c 
                               ON f.id_cliente = c.id_cliente
                           WHERE f.id_cliente = @idCliente
                           ORDER BY f.fecha_emision DESC";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@idCliente", idCliente);

            using MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Factura f = MappingFactura(dr);
                f.NombreCliente = dr["nombre_cliente"].ToString();
                f.ApellidosCliente = dr["apellidos_cliente"].ToString();
                f.NumeroDocumentoCliente = dr["documento_cliente"].ToString();
                lista.Add(f);
            }

            return lista;
        }

        /// <summary>
        /// Obtiene las facturas pendientes de pago.
        /// </summary>
        /// <returns>Lista de facturas pendientes.</returns>
        public List<Factura> ObtenerFacturasPendientes()
        {
            List<Factura> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT f.*,
                                  c.nombre AS nombre_cliente, 
                                  c.apellidos AS apellidos_cliente
                           FROM facturas f INNER JOIN clientes c 
                               ON f.id_cliente = c.id_cliente
                           WHERE f.estado = 'Pendiente'
                           ORDER BY f.fecha_emision ASC";

            MySqlCommand cmd = new(sql, con);

            using MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Factura f = MappingFactura(dr);
                f.NombreCliente = dr["nombre_cliente"].ToString();
                f.ApellidosCliente = dr["apellidos_cliente"].ToString();
                lista.Add(f);
            }

            return lista;
        }

        /// <summary>
        /// Inserta una factura completa junto con sus líneas de detalle
        /// y actualiza el stock de productos si procede.
        /// </summary>
        /// <param name="f">Factura completa con sus detalles.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool InsertarFacturaCompleta(Factura f)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            using MySqlTransaction tra = con.BeginTransaction();

            try
            {
                f.BaseImponible = f.Detalles.Sum(d => d.Subtotal);
                f.IvaTotal = f.Detalles.Sum(d => d.IvaImporte);
                f.Total = f.Detalles.Sum(d => d.TotalLinea);

                // Insertar datos de la factura
                string sqlFactura = @"INSERT INTO facturas (id_cliente, numero_factura, estado, base_imponible, 
                                          iva_total, total, metodo_pago, fecha_emision, observaciones)
                                      VALUES (@idCliente, @numeroFactura, @estado, @baseImponible, @ivaTotal, 
                                           @total, @metodoPago, @fechaEmision, @observaciones)";

                MySqlCommand cmdF = new(sqlFactura, con, tra);
                CargarParametrosFactura(cmdF, f);
                cmdF.ExecuteNonQuery();

                int idFacturaGenerada = Convert.ToInt32(cmdF.LastInsertedId);

                // Insertar detalles de la factura
                string sqlDetalle = @"INSERT INTO detalles_factura (id_factura, id_concepto, 
                                        nombre_concepto, tipo, cantidad, precio_unitario, iva_porcentaje, 
                                        subtotal, iva_importe, total_linea)
                                    VALUES (@idFactura, @idConcepto, @nombreConcepto, @tipo, @cantidad, 
                                        @precioUnitario, @ivaPorcentaje, @subtotal, @ivaImporte, @totalLinea)";

                MySqlCommand cmdD = new(sqlDetalle, con, tra);
                CargarParametrosDetalle(cmdD);

                foreach (var det in f.Detalles)
                {
                    cmdD.Parameters["@idFactura"].Value = idFacturaGenerada;
                    cmdD.Parameters["@idConcepto"].Value = (object)det.IdConcepto ?? DBNull.Value;
                    cmdD.Parameters["@nombreConcepto"].Value = det.NombreConcepto;
                    cmdD.Parameters["@tipo"].Value = det.Tipo;
                    cmdD.Parameters["@cantidad"].Value = det.Cantidad;
                    cmdD.Parameters["@precioUnitario"].Value = det.PrecioUnitario;
                    cmdD.Parameters["@ivaPorcentaje"].Value = det.IvaPorcentaje;
                    cmdD.Parameters["@subtotal"].Value = det.Subtotal;
                    cmdD.Parameters["@ivaImporte"].Value = det.IvaImporte;
                    cmdD.Parameters["@totalLinea"].Value = det.TotalLinea;

                    cmdD.ExecuteNonQuery();

                    // Descontar stock de los productos
                    if (det.Tipo == "Producto" && det.IdConcepto > 0)
                    {
                        string sqlStock = @"UPDATE conceptos 
                                            SET stock = stock - @cantidad 
                                            WHERE id_concepto = @idConcepto AND stock IS NOT NULL";

                        MySqlCommand cmdS = new(sqlStock, con, tra);
                        cmdS.Parameters.AddWithValue("@cantidad", det.Cantidad);
                        cmdS.Parameters.AddWithValue("@idConcepto", det.IdConcepto);
                        cmdS.ExecuteNonQuery();
                    }
                }

                tra.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tra.Rollback();
                throw new Exception("Error al insertar factura: " + ex.Message);
            }
        }

        /// <summary>
        /// Anula una factura existente.
        /// </summary>
        /// <param name="idFactura">Identificador de la factura.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool AnularFactura(int idFactura)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"UPDATE facturas 
                           SET estado = 'Anulada',
                               observaciones = CONCAT(IFNULL(observaciones,''), ' [ANULADA]')
                           WHERE id_factura = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idFactura);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Actualiza el estado de una factura.
        /// </summary>
        /// <param name="idFactura">Identificador de la factura.</param>
        /// <param name="estado">Nuevo estado.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool ActualizarEstadoFactura(int idFactura, string estado)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"UPDATE facturas 
                           SET estado = @estado 
                           WHERE id_factura = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idFactura);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Obtiene los ingresos del mes actual.
        /// </summary>
        /// <returns>Total de ingresos del mes.</returns>
        public decimal ObtenerIngresosMes()
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT IFNULL(SUM(total),0)
                           FROM facturas
                           WHERE MONTH(fecha_emision)=MONTH(CURDATE()) 
                               AND YEAR(fecha_emision)=YEAR(CURDATE())
                               AND estado='Pagada'";

            MySqlCommand cmd = new(sql, con);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Calcula la deuda pendiente de un cliente.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <returns>Total de deuda pendiente.</returns>
        public decimal CalcularDeudaCliente(int idCliente)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT IFNULL(SUM(total),0)
                           FROM facturas
                           WHERE id_cliente=@id AND estado='Pendiente'";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idCliente);

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Obtiene el último número de factura generado en un año concreto.
        /// </summary>
        /// <param name="anioActual">Año de búsqueda.</param>
        /// <returns>Último número de factura del año o null si no existe.</returns>
        public string? ObtenerUltimoNumeroPorAnio(int anioActual)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"SELECT MAX(numero_factura)
                           FROM facturas
                           WHERE numero_factura LIKE CONCAT(@anio, '-%')";

            using MySqlCommand cmd = new(sql, con)
            {
                Parameters =
                {
                    new MySqlParameter("@anio", anioActual.ToString())
                }
            };

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return null;

            return result.ToString();
        }

        /// <summary>
        /// Carga los parámetros de la cabecera de factura.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="cita">Objeto factura con los datos.</param>
        private static void CargarParametrosFactura(MySqlCommand cmd, Factura f)
        {
            cmd.Parameters.AddWithValue("@idCliente", f.IdCliente);
            cmd.Parameters.AddWithValue("@numeroFactura", f.NumeroFactura);
            cmd.Parameters.AddWithValue("@estado", f.Estado ?? "Pendiente");
            cmd.Parameters.AddWithValue("@baseImponible", f.BaseImponible);
            cmd.Parameters.AddWithValue("@ivaTotal", f.IvaTotal);
            cmd.Parameters.AddWithValue("@total", f.Total);
            cmd.Parameters.AddWithValue("@metodoPago", f.MetodoPago);
            cmd.Parameters.AddWithValue("@fechaEmision", DateTime.Now);
            cmd.Parameters.AddWithValue("@observaciones", f.Observaciones as object ?? DBNull.Value);
        }

        /// <summary>
        /// Inicializa los parámetros del detalle de factura.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        private static void CargarParametrosDetalle(MySqlCommand cmd)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@idFactura", MySqlDbType.Int32);
            cmd.Parameters.Add("@idConcepto", MySqlDbType.Int32);
            cmd.Parameters.Add("@nombreConcepto", MySqlDbType.VarChar);
            cmd.Parameters.Add("@tipo", MySqlDbType.VarChar);
            cmd.Parameters.Add("@cantidad", MySqlDbType.Int32);
            cmd.Parameters.Add("@precioUnitario", MySqlDbType.Decimal);
            cmd.Parameters.Add("@ivaPorcentaje", MySqlDbType.Decimal);
            cmd.Parameters.Add("@subtotal", MySqlDbType.Decimal);
            cmd.Parameters.Add("@ivaImporte", MySqlDbType.Decimal);
            cmd.Parameters.Add("@totalLinea", MySqlDbType.Decimal);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Factura.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Factura.</returns>
        private static Factura MappingFactura(MySqlDataReader rdr)
        {
            return new Factura
            {
                IdFactura = Convert.ToInt32(rdr["id_factura"]),
                IdCliente = Convert.ToInt32(rdr["id_cliente"]),
                NumeroFactura = rdr["numero_factura"].ToString(),
                Estado = rdr["estado"].ToString(),
                FechaEmision = Convert.ToDateTime(rdr["fecha_emision"]),
                BaseImponible = Convert.ToDecimal(rdr["base_imponible"]),
                IvaTotal = Convert.ToDecimal(rdr["iva_total"]),
                Total = Convert.ToDecimal(rdr["total"]),
                MetodoPago = rdr["metodo_pago"].ToString(),
                Observaciones = rdr["observaciones"] == DBNull.Value ? "" : rdr["observaciones"].ToString()
            };
        }
    }
}