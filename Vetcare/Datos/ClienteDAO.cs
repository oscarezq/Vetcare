using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada de realizar las operaciones de acceso a datos relacionadas con 
    /// la entidad Cliente en la base de datos vetcare.
    /// </summary>
    public class ClienteDAO
    {
        // Objeto para obtener la conexión
        Conexion conexion = new Conexion();

        /// <summary>
        /// Método para obtener una lista con todos los clientes que hay en la tabla clientes 
        /// de la base de datos vetcare.
        /// </summary>
        /// <returns>Lista con todos los clientes</returns>
        public List<Cliente> ObtenerTodos()
        {
            // Lista con todos los clientes
            List<Cliente> lista = new List<Cliente>();

            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para obtener todos los clientes
                string sql = "SELECT id_cliente, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta " +
                             "FROM clientes";

                // Generamos el comando
                MySqlCommand cmd = new MySqlCommand(sql, con);

                // Ejecutamos la consulta con ExecuteReader para obtener un MySqlDataReader
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    // Mientras haya registros en el MySqlDataReader, los leemos
                    while (rdr.Read())
                    {
                        // Creamos un nuevo ciente con los datos leidos
                        Cliente cliente = new Cliente
                        {
                            IdCliente = Convert.ToInt32(rdr["id_cliente"]),
                            NumDocumento = rdr["num_documento"].ToString(),
                            Nombre = rdr["nombre"].ToString(),
                            Apellidos = rdr["apellidos"].ToString(),
                            Telefono = rdr["telefono"].ToString(),
                            Email = rdr["email"].ToString(),
                            Direccion = rdr["direccion"].ToString(),
                            FechaAlta = Convert.ToDateTime(rdr["fecha_alta"])
                        };

                        // Añadimos a la lista el cliente recién creado
                        lista.Add(cliente);
                    }
                }
            }

            // Devolvemos la lista
            return lista;
        }

        /// <summary>
        /// Obtiene un cliente de la base de datos según su ID.
        /// </summary>
        /// <param name="id">ID del cliente a buscar.</param>
        /// <returns>Cliente si existe; de lo contrario, null.</returns>
        public Cliente ObtenerPorId(int id)
        {
            // Cliente a buscar
            Cliente cliente = null;

            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para obtener el cliente
                string query = "SELECT id_cliente, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta " +
                               "FROM clientes WHERE id_cliente = @id_cliente";

                // Generamos el comando y le añadimos el parámetro
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id_cliente", id);

                // Ejecutamos la consulta con ExecuteReader para obtener un MySqlDataReader
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // Recorremos los resultados
                    if (reader.Read())
                    {
                        // Creamos un nuevo ciente con los datos leidos
                        cliente = new Cliente
                        {
                            IdCliente = Convert.ToInt32(reader["id_cliente"]),
                            NumDocumento = reader["num_documento"].ToString(),
                            Nombre = reader["nombre"].ToString(),
                            Apellidos = reader["apellidos"].ToString(),
                            Telefono = reader["telefono"].ToString(),
                            Email = reader["email"].ToString(),
                            Direccion = reader["direccion"].ToString(),
                            FechaAlta = Convert.ToDateTime(reader["fecha_alta"])
                        };
                    }
                }
            }

            // Devolvemos el cliente
            return cliente;
        }

        /// <summary>
        /// Método para insertar un nuevo cliente en la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="cliente">Cliente que se va a insertar</param>
        /// <returns>Booleano que indica si se ha insertado correctamente el cliente en la base de datos</returns>
        public bool Insertar(Cliente cliente)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para insertar cliente
                string sql = "INSERT INTO clientes (num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta) " +
                             "VALUES (@dni, @nom, @ape, @tel, @mail, @dir, @fec)";

                // Creamos el comando y le añadimos los parámetros del cliente
                MySqlCommand cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@dni", cliente.NumDocumento);
                cmd.Parameters.AddWithValue("@nom", cliente.Nombre);
                cmd.Parameters.AddWithValue("@ape", cliente.Apellidos);
                cmd.Parameters.AddWithValue("@tel", cliente.Telefono);
                cmd.Parameters.AddWithValue("@mail", cliente.Email);
                cmd.Parameters.AddWithValue("@dir", cliente.Direccion);
                cmd.Parameters.AddWithValue("@fec", cliente.FechaAlta);

                // Ejecutamos la consulta con ExecuteNonQuery y obtenemos el número de filas afectadas
                int resultado = cmd.ExecuteNonQuery();

                // Si el resultado no es cero, devolvemos true (se ha insertado el cliente)
                if (resultado > 0)
                    return true;

                // Si el resultado es menor que cero, devolvemos false (NO se ha insertado el cliente)
                return false;
            }
        }

        /// <summary>
        /// Método para insertar varios clientes en la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="clientes">Lista de clientes que se van a insertar</param>
        /// <returns>Booleano que indica si se han insertado correctamente los clientes en la base de datos</returns>
        public bool InsertarVarios(List<Cliente> clientes)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión e iniciamos una transacción
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Consulta para insertar cliente
                    string sql = "INSERT INTO clientes (num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta) " +
                                 "VALUES (@numDocumento, @nom, @ape, @tel, @mail, @dir, @fec)";

                    // Recorremos la lista de clientes
                    foreach (Cliente cliente in clientes)
                    {
                        // Creamos el comando y le añadimos los parámetros del cliente
                        MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);
                        cmd.Parameters.AddWithValue("@numDocumento", cliente.NumDocumento);
                        cmd.Parameters.AddWithValue("@nom", cliente.Nombre);
                        cmd.Parameters.AddWithValue("@ape", cliente.Apellidos);
                        cmd.Parameters.AddWithValue("@tel", cliente.Telefono);
                        cmd.Parameters.AddWithValue("@mail", cliente.Email);
                        cmd.Parameters.AddWithValue("@dir", cliente.Direccion);
                        cmd.Parameters.AddWithValue("@fec", cliente.FechaAlta);

                        // Ejecutamos la consulta con ExecuteNonQuery
                        cmd.ExecuteNonQuery();
                    }

                    // Si todo ha ido bien hacemos commit
                    transaccion.Commit();
                }
                catch
                {
                    // Si algo falla hacemos rollback
                    transaccion.Rollback();
                    return false;
                }
            }

            // Si todo ha ido bien devolvemos true
            return true;
        }

        /// <summary>
        /// Método para eliminar un cliente de la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente que se desea eliminar</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente el cliente de la base de datos</returns>
        public bool Eliminar(int idCliente)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para eliminar cliente con ese identificador
                string query = "DELETE FROM clientes WHERE id_cliente = @id";

                // Cremos el comando y le añadimos el parámetro
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idCliente);

                // Ejecutamos la consulta con ExecuteNonQuery y obtenemos el número de filas afectadas
                int resultado = cmd.ExecuteNonQuery();

                // Si el resultado no es cero, devolvemos true (se ha borrado el cliente)
                if (resultado > 0)
                    return true;

                // Si el resultado es menor que cero, devolvemos false (NO se ha borrado el cliente)
                return false;
            }
        }

        /// <summary>
        /// Método para eliminar varios clientes de la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="idsClientes">Lista de identificadores de los clientes a eliminar</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente los clientes de la base de datos</returns>
        public bool EliminarVarios(List<int> idsClientes)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión e iniciamos una transacción
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Recorremos todos los identificadores que hay en la lista
                    foreach (int id in idsClientes)
                    {
                        // Consulta para eliminar el cliente con ese identificador
                        string query = "DELETE FROM clientes WHERE id_cliente = @id";

                        // Cremos el comando y le añadimos el parámetro
                        MySqlCommand cmd = new MySqlCommand(query, con, transaccion);
                        cmd.Parameters.AddWithValue("@id", id);

                        // Ejecutamos la consulta con ExecuteNonQuery
                        cmd.ExecuteNonQuery();
                    }

                    // Si todo ha ido bien hacemos commit
                    transaccion.Commit();
                }
                catch
                {
                    // Si algo ha fallado, hacemos rollback
                    transaccion.Rollback();
                    return false;
                }
            }

            // Si todo ha ido bien devolvemos true
            return true;
        }

        /// <summary>
        /// Método para actualizar los datos de un cliente existente en la tabla clientes de la base de datos vetcare.
        /// </summary>
        /// <param name="cliente">Objeto cliente con los nuevos datos</param>
        /// <returns>Booleano que indica si la actualización se realizó correctamente en la base de datos</returns>
        public bool Actualizar(Cliente cliente)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para actualizar cliente
                string query = @"UPDATE clientes 
                                 SET num_documento = @numDocumento,
                                     nombre = @nombre,
                                     apellidos = @apellidos,
                                     telefono = @telefono,
                                     email = @email,
                                     direccion = @direccion
                                 WHERE id_cliente = @id";

                // Cremos el comando y le añadimos el parámetro
                using (MySqlCommand comando = new MySqlCommand(query, con))
                {
                    comando.Parameters.AddWithValue("@numDocumento", cliente.NumDocumento);
                    comando.Parameters.AddWithValue("@nombre", cliente.Nombre);
                    comando.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
                    comando.Parameters.AddWithValue("@telefono", cliente.Telefono);
                    comando.Parameters.AddWithValue("@email", cliente.Email);
                    comando.Parameters.AddWithValue("@direccion", cliente.Direccion);
                    comando.Parameters.AddWithValue("@id", cliente.IdCliente);

                    // Ejecutamos la consulta con ExecuteNonQuery para obtener las filas afectadas
                    int filasAfectadas = comando.ExecuteNonQuery();

                    // Si el resultado no es cero, devolvemos true (se ha actualizado el cliente)
                    if (filasAfectadas > 0)
                        return true;

                    // Si el resultado es menor que cero, devolvemos false (NO se ha actualizado el cliente)
                    return false;
                }
            }
        }

        /// <summary>
        /// Actualiza múltiples clientes en una sola transacción.
        /// Si ocurre un error, se revierte toda la operación.
        /// </summary>
        /// <param name="clientes">Lista de clientes a actualizar.</param>
        /// <returns>True si todos los clientes se actualizaron correctamente.</returns>
        public bool ActualizarVarios(List<Cliente> clientes)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión e iniciamos una transacción
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Recorremos todos los clientes de la lista de clientes
                    foreach (Cliente cliente in clientes)
                    {
                        // Consulta para actualizar clientes
                        string query = @"UPDATE clientes 
                                         SET num_documento = @numDocumento,
                                             nombre = @nombre,
                                             apellidos = @apellidos,
                                             telefono = @telefono,
                                             email = @email,
                                             direccion = @direccion
                                         WHERE id_cliente = @id";

                        // Cremos el comando y le añadimos el parámetro
                        using (MySqlCommand comando = new MySqlCommand(query, con, transaccion))
                        {
                            comando.Parameters.AddWithValue("@numDocumento", cliente.NumDocumento);
                            comando.Parameters.AddWithValue("@nombre", cliente.Nombre);
                            comando.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
                            comando.Parameters.AddWithValue("@telefono", cliente.Telefono);
                            comando.Parameters.AddWithValue("@email", cliente.Email);
                            comando.Parameters.AddWithValue("@direccion", cliente.Direccion);
                            comando.Parameters.AddWithValue("@id", cliente.IdCliente);

                            // Ejecutamos el comando para actualizar el cliente
                            comando.ExecuteNonQuery();
                        }
                    }

                    // Si se han actualizado todos los clientes, hacemos un commit
                    transaccion.Commit();
                }
                catch
                {
                    // Si algo ha fallado, hacemos un rollback para que no se actualice ningún cliente
                    transaccion.Rollback();
                    // Devolvemos false porque algo ha fallado
                    return false;
                }
            }

            // Si ha ido todo bien, devolvemos true
            return true;
        }
    }
}
