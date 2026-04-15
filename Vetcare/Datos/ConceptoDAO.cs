using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Vetcare.Entidades;
using Vetcare.Utilidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Concepto.
    /// Gestiona las operaciones de consulta, inserción, actualización y eliminación
    /// de conceptos (productos y servicios) en la base de datos.
    /// </summary>
    public class ConceptoDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todos los conceptos registrados en el sistema.
        /// </summary>
        /// <returns>Lista de conceptos.</returns>
        public List<Concepto> ObtenerTodos()
        {
            List<Concepto> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "SELECT * " +
                         "FROM conceptos " +
                         "ORDER BY nombre ASC";

            MySqlCommand cmd = new(sql, con);
            using MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(MapearConcepto(dr));

            return lista;
        }

        /// <summary>
        /// Obtiene un concepto por su identificador.
        /// </summary>
        /// <param name="id">Identificador del concepto.</param>
        /// <returns>Objeto Concepto si existe; en caso contrario, null.</returns>
        public Concepto? ObtenerPorId(int id)
        {
            Concepto? concepto = null;

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "SELECT * " +
                         "FROM conceptos " +
                         "WHERE id_concepto = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
                concepto = MapearConcepto(dr);

            return concepto;
        }

        /// <summary>
        /// Obtiene todos los servicios registrados en el sistema.
        /// </summary>
        /// <returns>Lista de servicios.</returns>
        public List<Concepto> ObtenerServicios()
        {
            List<Concepto> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "SELECT *" +
                         "FROM conceptos " +
                         "WHERE tipo = 'Servicio' " +
                         "ORDER BY nombre ASC";

            MySqlCommand cmd = new(sql, con);
            using MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(MapearConcepto(dr));

            return lista;
        }

        /// <summary>
        /// Obtiene todos los productos registrados en el sistema.
        /// </summary>
        /// <returns>Lista de productos.</returns>
        public List<Concepto> ObtenerProductos()
        {
            List<Concepto> lista = new();

            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "SELECT * " +
                         "FROM conceptos " +
                         "WHERE tipo = 'Producto' " +
                         "ORDER BY nombre ASC";
            
            MySqlCommand cmd = new(sql, con);
            using MySqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(MapearConcepto(dr));

            return lista;
        }

        /// <summary>
        /// Inserta un nuevo concepto en la base de datos.
        /// </summary>
        /// <param name="c">Objeto concepto a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(Concepto c)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"INSERT INTO conceptos 
                          (tipo, nombre, descripcion, precio, iva_porcentaje, stock, activo)
                          VALUES (@tipo, @nombre, @descripcion, @precio, @iva, @stock, @activo)";

            MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, c);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Actualiza los datos de un concepto existente.
        /// </summary>
        /// <param name="c">Objeto concepto con los datos actualizados.</param>
        /// <returns>True si la actualización se realiza correctamente.</returns>
        public bool Actualizar(Concepto c)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = @"UPDATE conceptos 
                           SET tipo = @tipo,
                               nombre = @nombre,
                               descripcion = @descripcion,
                               precio = @precio,
                               iva_porcentaje = @iva,
                               stock = @stock,
                               activo = @activo
                           WHERE id_concepto = @id";

            MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, c);
            cmd.Parameters.AddWithValue("@id", c.IdConcepto);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Desactiva un concepto en el sistema (borrado lógico).
        /// </summary>
        /// <param name="idConcepto">Identificador del concepto.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool Eliminar(int idConcepto)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "UPDATE conceptos " +
                         "SET activo = FALSE " +
                         "WHERE id_concepto = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idConcepto);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Reactiva un concepto previamente desactivado.
        /// </summary>
        /// <param name="idConcepto">Identificador del concepto.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool Reactivar(int idConcepto)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "UPDATE conceptos " +
                         "SET activo = TRUE " +
                         "WHERE id_concepto = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idConcepto);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto Concepto.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="c">Objeto concepto con los datos.</param>
        private static void CargarParametros(MySqlCommand cmd, Concepto c)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@tipo", c.Tipo);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)c.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", c.Precio);
            cmd.Parameters.AddWithValue("@iva", c.IvaPorcentaje);
            cmd.Parameters.AddWithValue("@stock", c.Stock.HasValue ? c.Stock : DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", c.Activo);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Concepto.
        /// </summary>
        /// <param name="dr">Lector de datos.</param>
        /// <returns>Objeto Concepto.</returns>
        private static Concepto MapearConcepto(MySqlDataReader dr)
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