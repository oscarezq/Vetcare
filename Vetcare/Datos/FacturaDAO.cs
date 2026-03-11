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
                        // 1. Insertar Cabecera
                        string sqlFactura = @"INSERT INTO facturas (id_cliente, numero_factura, estado, total, metodo_pago, observaciones, fecha_emision) 
                                    VALUES (@cli, @num, @est, @tot, @met, @obs, @fec)";

                        using (MySqlCommand cmdF = new MySqlCommand(sqlFactura, con, tra))
                        {
                            cmdF.Parameters.AddWithValue("@cli", f.IdCliente);
                            cmdF.Parameters.AddWithValue("@num", f.NumeroFactura);
                            cmdF.Parameters.AddWithValue("@est", f.Estado ?? "Pendiente");
                            cmdF.Parameters.AddWithValue("@tot", f.Total);
                            cmdF.Parameters.AddWithValue("@met", f.MetodoPago);
                            cmdF.Parameters.AddWithValue("@obs", (object)f.Observaciones ?? DBNull.Value);
                            cmdF.Parameters.AddWithValue("@fec", DateTime.Now);

                            cmdF.ExecuteNonQuery();

                            // Convertir de long a int de forma segura
                            int idFacturaGenerada = Convert.ToInt32(cmdF.LastInsertedId);

                            // 2. Insertar Detalles (Reutilizando el comando)
                            string sqlDetalle = @"INSERT INTO detalles_factura (id_factura, id_servicio, cantidad, precio_unitario) 
                                        VALUES (@idF, @idS, @cant, @pre)";

                            using (MySqlCommand cmdD = new MySqlCommand(sqlDetalle, con, tra))
                            {
                                // Añadimos los parámetros una sola vez (vacíos)
                                cmdD.Parameters.Add("@idF", MySqlDbType.Int32);
                                cmdD.Parameters.Add("@idS", MySqlDbType.Int32);
                                cmdD.Parameters.Add("@cant", MySqlDbType.Int32);
                                cmdD.Parameters.Add("@pre", MySqlDbType.Decimal);

                                foreach (var det in f.Detalles)
                                {
                                    // Asignamos valores en cada vuelta del bucle
                                    cmdD.Parameters["@idF"].Value = idFacturaGenerada;
                                    cmdD.Parameters["@idS"].Value = det.IdServicio;
                                    cmdD.Parameters["@cant"].Value = det.Cantidad;
                                    cmdD.Parameters["@pre"].Value = det.PrecioUnitario;

                                    cmdD.ExecuteNonQuery();
                                }
                            }
                        }

                        tra.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        // Loguear el error si tienes un logger
                        throw new Exception("Error en la base de datos: " + ex.Message);
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

        private Factura MapearFactura(MySqlDataReader dr)
        {
            return new Factura
            {
                IdFactura = Convert.ToInt32(dr["id_factura"]),
                IdCliente = Convert.ToInt32(dr["id_cliente"]),
                NumeroFactura = dr["numero_factura"].ToString(),
                Estado = dr["estado"].ToString(),
                FechaEmision = Convert.ToDateTime(dr["fecha_emision"]),
                Total = Convert.ToDecimal(dr["total"]),
                MetodoPago = dr["metodo_pago"].ToString(),
                Observaciones = dr["observaciones"].ToString()
            };
        }
    }
}