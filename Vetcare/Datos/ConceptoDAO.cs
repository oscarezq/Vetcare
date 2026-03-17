using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class ConceptoDAO
    {
        private Conexion conexion = new Conexion();

        public List<Concepto> ObtenerTodos()
        {
            List<Concepto> lista = new List<Concepto>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM conceptos ORDER BY nombre ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            lista.Add(MapearConcepto(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener conceptos: " + ex.Message);
                }
            }
            return lista;
        }

        public List<Concepto> ObtenerServicios()
        {
            List<Concepto> lista = new List<Concepto>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Filtramos por tipo y solo los activos para la venta
                string sql = "SELECT * FROM conceptos WHERE tipo = 'Servicio' AND activo = 1 ORDER BY nombre ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            lista.Add(MapearConcepto(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener servicios: " + ex.Message);
                }
            }
            return lista;
        }

        public List<Concepto> ObtenerProductos()
        {
            List<Concepto> lista = new List<Concepto>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM conceptos WHERE tipo = 'Producto' AND activo = 1 ORDER BY nombre ASC";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                            lista.Add(MapearConcepto(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener productos: " + ex.Message);
                }
            }
            return lista;
        }

        public bool Insertar(Concepto c)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"INSERT INTO conceptos 
                               (tipo, nombre, descripcion, precio_base, iva_porcentaje, stock, activo) 
                               VALUES 
                               (@tipo, @nom, @desc, @precio, @iva, @stock, @activo)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tipo", c.Tipo);
                cmd.Parameters.AddWithValue("@nom", c.Nombre);
                cmd.Parameters.AddWithValue("@desc", c.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precio", c.PrecioBase);
                cmd.Parameters.AddWithValue("@iva", c.IvaPorcentaje);
                cmd.Parameters.AddWithValue("@stock", c.Stock.HasValue ? c.Stock : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@activo", c.Activo);

                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al insertar concepto: " + ex.Message);
                }
            }
        }

        public bool Actualizar(Concepto c)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = @"UPDATE conceptos 
                               SET tipo = @tipo,
                                   nombre = @nom,
                                   descripcion = @desc,
                                   precio_base = @precio,
                                   iva_porcentaje = @iva,
                                   stock = @stock,
                                   activo = @activo
                               WHERE id_concepto = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", c.IdConcepto);
                cmd.Parameters.AddWithValue("@tipo", c.Tipo);
                cmd.Parameters.AddWithValue("@nom", c.Nombre);
                cmd.Parameters.AddWithValue("@desc", c.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precio", c.PrecioBase);
                cmd.Parameters.AddWithValue("@iva", c.IvaPorcentaje);
                cmd.Parameters.AddWithValue("@stock", c.Stock.HasValue ? c.Stock : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@activo", c.Activo);

                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar concepto: " + ex.Message);
                }
            }
        }

        public bool Eliminar(int idConcepto)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "DELETE FROM conceptos WHERE id_concepto = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idConcepto);
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    // Nota: Si el concepto ya está en una factura, esto fallará por FK.
                    // Podrías considerar un "Borrado Lógico" cambiando Activo = 0.
                    throw new Exception("Error al eliminar: " + ex.Message);
                }
            }
        }

        private Concepto MapearConcepto(MySqlDataReader dr)
        {
            return new Concepto
            {
                IdConcepto = Convert.ToInt32(dr["id_concepto"]),
                Tipo = dr["tipo"].ToString(),
                Nombre = dr["nombre"].ToString(),
                Descripcion = dr["descripcion"] == DBNull.Value ? null : dr["descripcion"].ToString(),
                PrecioBase = Convert.ToDecimal(dr["precio_base"]),
                IvaPorcentaje = Convert.ToDecimal(dr["iva_porcentaje"]),
                Stock = dr["stock"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["stock"]),
                Activo = Convert.ToBoolean(dr["activo"]),
                FechaAlta = Convert.ToDateTime(dr["fecha_alta"])
            };
        }
    }
}