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

        public Concepto ObtenerPorId(int id)
        {
            Concepto concepto = null;
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "SELECT * FROM conceptos WHERE id_concepto = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            concepto = MapearConcepto(dr);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el concepto por ID: " + ex.Message);
                }
            }

            return concepto;
        }

        public List<Concepto> ObtenerServicios()
        {
            List<Concepto> lista = new List<Concepto>();
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Filtramos por tipo y solo los activos para la venta
                string sql = "SELECT * FROM conceptos WHERE tipo = 'Servicio' ORDER BY nombre ASC";
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
                string sql = "SELECT * FROM conceptos WHERE tipo = 'Producto' ORDER BY nombre ASC";
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
                               (tipo, nombre, descripcion, precio, iva_porcentaje, stock, activo) 
                               VALUES 
                               (@tipo, @nom, @desc, @precio, @iva, @stock, @activo)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@tipo", c.Tipo);
                cmd.Parameters.AddWithValue("@nom", c.Nombre);
                cmd.Parameters.AddWithValue("@desc", c.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precio", c.Precio);
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
                                   precio = @precio,
                                   iva_porcentaje = @iva,
                                   stock = @stock,
                                   activo = @activo
                               WHERE id_concepto = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", c.IdConcepto);
                cmd.Parameters.AddWithValue("@tipo", c.Tipo);
                cmd.Parameters.AddWithValue("@nom", c.Nombre);
                cmd.Parameters.AddWithValue("@desc", c.Descripcion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@precio", c.Precio);
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
                con.Open();

                string sql = "UPDATE conceptos SET activo = FALSE WHERE id_concepto = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idConcepto);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ActualizarStock(int id, int nuevoStock)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                string sql = "UPDATE conceptos SET stock = @stock WHERE id_concepto = @id";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@stock", nuevoStock);
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al actualizar stock: " + ex.Message);
                }
            }
        }

        public bool Reactivar(int idConcepto)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "UPDATE conceptos SET activo = TRUE WHERE id_concepto = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idConcepto);

                return cmd.ExecuteNonQuery() > 0;
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
                Precio = Convert.ToDecimal(dr["precio"]),
                IvaPorcentaje = Convert.ToDecimal(dr["iva_porcentaje"]),
                Stock = dr["stock"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["stock"]),
                Activo = Convert.ToBoolean(dr["activo"]),
                FechaAlta = Convert.ToDateTime(dr["fecha_alta"])
            };
        }
    }
}