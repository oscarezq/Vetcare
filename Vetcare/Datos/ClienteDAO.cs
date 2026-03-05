using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    public class ClienteDAO
    {
        private Conexion conexion = new Conexion();

        // ------------------ MÉTODOS PRINCIPALES ------------------

        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> lista = new List<Cliente>();

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta
                               FROM clientes";
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        lista.Add(MappingCliente(rdr));
                }
            }

            return lista;
        }

        public Cliente ObtenerPorId(int id)
        {
            Cliente cliente = null;

            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta
                               FROM clientes
                               WHERE id_cliente = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                            cliente = MappingCliente(rdr);
                    }
                }
            }

            return cliente;
        }

        public bool Insertar(Cliente cliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"INSERT INTO clientes
                               (tipo_documento, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta)
                               VALUES (@tipoDocumento, @numDocumento, @nombre, @apellidos, @telefono, @email, @direccion, @fechaAlta)";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    CargarParametros(cmd, cliente);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool InsertarVarios(List<Cliente> clientes)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string sql = @"INSERT INTO clientes
                                   (tipo_documento, num_documento, nombre, apellidos, telefono, email, direccion, fecha_alta)
                                   VALUES (@tipoDocumento, @numDocumento, @nombre, @apellidos, @telefono, @email, @direccion, @fechaAlta)";

                    foreach (Cliente cliente in clientes)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con, transaccion))
                        {
                            CargarParametros(cmd, cliente);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    return false;
                }
            }
        }

        public bool Actualizar(Cliente cliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = @"UPDATE clientes 
                               SET tipo_documento = @tipoDocumento,
                                   num_documento = @numDocumento,
                                   nombre = @nombre,
                                   apellidos = @apellidos,
                                   telefono = @telefono,
                                   email = @email,
                                   direccion = @direccion
                               WHERE id_cliente = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    CargarParametros(cmd, cliente);
                    cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarVarios(List<Cliente> clientes)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string sql = @"UPDATE clientes 
                                   SET tipo_documento = @tipoDocumento,
                                       num_documento = @numDocumento,
                                       nombre = @nombre,
                                       apellidos = @apellidos,
                                       telefono = @telefono,
                                       email = @email,
                                       direccion = @direccion
                                   WHERE id_cliente = @id";

                    foreach (Cliente cliente in clientes)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con, transaccion))
                        {
                            CargarParametros(cmd, cliente);
                            cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    return false;
                }
            }
        }

        public bool Eliminar(int idCliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = "DELETE FROM clientes WHERE id_cliente = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", idCliente);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarVarios(List<int> idsClientes)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    string sql = "DELETE FROM clientes WHERE id_cliente = @id";

                    foreach (int id in idsClientes)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(sql, con, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    return false;
                }
            }
        }

        // ------------------ MÉTODOS AUXILIARES ------------------

        private Cliente MappingCliente(MySqlDataReader rdr)
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
                Direccion = rdr["direccion"].ToString(),
                FechaAlta = Convert.ToDateTime(rdr["fecha_alta"])
            };
        }

        private void CargarParametros(MySqlCommand cmd, Cliente cliente)
        {
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@tipoDocumento", cliente.TipoDocumento);
            cmd.Parameters.AddWithValue("@numDocumento", cliente.NumDocumento);
            cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
            cmd.Parameters.AddWithValue("@apellidos", cliente.Apellidos);
            cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
            cmd.Parameters.AddWithValue("@email", cliente.Email);
            cmd.Parameters.AddWithValue("@direccion", cliente.Direccion);
            cmd.Parameters.AddWithValue("@fechaAlta", cliente.FechaAlta);
        }
    }
}