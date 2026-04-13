using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Objeto de acceso a datos (DAO) para la entidad Cliente.
    /// Gestiona las operaciones de consulta, inserción, actualización y desactivación
    /// de clientes en la base de datos.
    /// </summary>
    public class ClienteDAO
    {
        /// <summary>
        /// Objeto encargado de proporcionar la conexión a la base de datos.
        /// </summary>
        private readonly Conexion conexion = new();

        /// <summary>
        /// Obtiene todos los clientes registrados en el sistema.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> lista = new();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, activo, telefono, email, 
                                      calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta
                               FROM clientes";

                using MySqlCommand cmd = new(sql, con);
                using MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                    lista.Add(MappingCliente(rdr));
            }

            return lista;
        }

        /// <summary>
        /// Obtiene un cliente por su identificador.
        /// </summary>
        /// <param name="id">Identificador del cliente.</param>
        /// <returns>Objeto Cliente si existe; en caso contrario, null.</returns>
        public Cliente? ObtenerPorId(int id)
        {
            Cliente? cliente = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, activo, telefono, email, 
                                      calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta
                               FROM clientes
                               WHERE id_cliente = @id";

                using MySqlCommand cmd = new(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    cliente = MappingCliente(rdr);
            }

            return cliente;
        }

        /// <summary>
        /// Inserta un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto cliente a insertar.</param>
        /// <returns>True si la inserción se realiza correctamente.</returns>
        public bool Insertar(Cliente cliente)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @"INSERT INTO clientes
                               (tipo_documento, num_documento, nombre, apellidos, telefono, email, 
                                calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta)
                               VALUES (@tipoDocumento, @numDocumento, @nombre, @apellidos, @telefono, @email, 
                                @calle, @numero, @pisoPuerta, @codigoPostal, @localidad, @provincia, @fechaAlta)";

            using MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, cliente);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Actualiza los datos de un cliente existente.
        /// </summary>
        /// <param name="cliente">Objeto cliente con los datos actualizados.</param>
        /// <returns>True si la actualización se realiza correctamente.</returns>
        public bool Actualizar(Cliente cliente)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = @"UPDATE clientes 
                               SET tipo_documento = @tipoDocumento,
                                   num_documento = @numDocumento,
                                   nombre = @nombre,
                                   apellidos = @apellidos,
                                   telefono = @telefono,
                                   email = @email,
                                   calle = @calle,
                                   numero = @numero,
                                   piso_puerta = @pisoPuerta,
                                   codigo_postal = @codigoPostal,
                                   localidad = @localidad,
                                   provincia = @provincia
                               WHERE id_cliente = @id";

            using MySqlCommand cmd = new(sql, con);
            CargarParametros(cmd, cliente);
            cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Desactiva un cliente y todas sus mascotas asociadas mediante una transacción.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool Desactivar(int idCliente)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            using MySqlTransaction trans = con.BeginTransaction();
            try
            {
                string sqlCliente = "UPDATE clientes SET activo = FALSE WHERE id_cliente = @id";
                using (MySqlCommand cmdCliente = new(sqlCliente, con, trans))
                {
                    cmdCliente.Parameters.AddWithValue("@id", idCliente);
                    cmdCliente.ExecuteNonQuery();
                }

                string sqlMascotas = "UPDATE mascotas SET activo = FALSE WHERE id_cliente = @id";
                using (MySqlCommand cmdMascotas = new(sqlMascotas, con, trans))
                {
                    cmdMascotas.Parameters.AddWithValue("@id", idCliente);
                    cmdMascotas.ExecuteNonQuery();
                }

                trans.Commit();
                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Reactiva un cliente previamente desactivado.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool Reactivar(int idCliente)
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();

            string sql = "UPDATE clientes SET activo = TRUE WHERE id_cliente = @id";

            MySqlCommand cmd = new(sql, con);
            cmd.Parameters.AddWithValue("@id", idCliente);

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Cuenta el número total de clientes registrados en el sistema.
        /// </summary>
        /// <returns>Número de clientes.</returns>
        public int ContarClientes()
        {
            using MySqlConnection con = conexion.ObtenerConexion();
            con.Open();
            string sql = "SELECT COUNT(*) FROM clientes";
            MySqlCommand cmd = new(sql, con);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Carga los parámetros necesarios en un comando SQL a partir de un objeto Cliente.
        /// </summary>
        /// <param name="cmd">Comando MySQL.</param>
        /// <param name="cliente">Objeto cliente con los datos.</param>
        private static void CargarParametros(MySqlCommand cmd, Cliente cliente)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@tipoDocumento", cliente.TipoDocumento);
            cmd.Parameters.AddWithValue("@numDocumento", cliente.NumDocumento);
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
            cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
            cmd.Parameters.AddWithValue("@email", cliente.Email);
            cmd.Parameters.AddWithValue("@calle", cliente.CalleDireccion);
            cmd.Parameters.AddWithValue("@numero", cliente.NumeroDireccion);
            cmd.Parameters.AddWithValue("@pisoPuerta", cliente.PisoPuertaDireccion);
            cmd.Parameters.AddWithValue("@codigoPostal", cliente.CodigoPostalDireccion);
            cmd.Parameters.AddWithValue("@localidad", cliente.LocalidadDireccion);
            cmd.Parameters.AddWithValue("@provincia", cliente.ProvinciaDireccion);
            cmd.Parameters.AddWithValue("@fechaAlta", cliente.FechaAlta);
        }

        /// <summary>
        /// Realiza el mapeo de un registro de base de datos a un objeto Cliente.
        /// </summary>
        /// <param name="rdr">Lector de datos.</param>
        /// <returns>Objeto Cliente.</returns>
        private static Cliente MappingCliente(MySqlDataReader rdr)
        {
            return new Cliente
            {
                IdCliente = Convert.ToInt32(rdr["id_cliente"]),
                TipoDocumento = rdr["tipo_documento"].ToString(),
                NumDocumento = rdr["num_documento"].ToString(),
                Nombre = rdr["nombre"].ToString(),
                Apellidos = rdr["apellidos"].ToString(),
                Telefono = rdr["telefono"].ToString(),
                Email = rdr["email"].ToString(),
                Activo = Convert.ToBoolean(rdr["activo"]),
                CalleDireccion = rdr["calle"].ToString(),
                NumeroDireccion = rdr["numero"].ToString(),
                PisoPuertaDireccion = rdr["piso_puerta"].ToString(),
                CodigoPostalDireccion = rdr["codigo_postal"].ToString(),
                LocalidadDireccion = rdr["localidad"].ToString(),
                ProvinciaDireccion = rdr["provincia"].ToString(),
                FechaAlta = Convert.ToDateTime(rdr["fecha_alta"])
            };
        }
    }
}