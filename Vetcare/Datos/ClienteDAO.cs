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
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, activo, telefono, email, 
                                      calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta
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
                string sql = @"SELECT id_cliente, tipo_documento, num_documento, nombre, apellidos, activo, telefono, email, 
                                      calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta
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
                               (tipo_documento, num_documento, nombre, apellidos, telefono, email, 
                                calle, numero, piso_puerta, codigo_postal, localidad, provincia, fecha_alta)
                               VALUES (@tipoDocumento, @numDocumento, @nombre, @apellidos, @telefono, @email, 
                                @calle, @numero, @pisoPuerta, @codigoPostal, @localidad, @provincia, @fechaAlta)";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    CargarParametros(cmd, cliente);
                    return cmd.ExecuteNonQuery() > 0;
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
                                   calle = @calle,
                                   numero = @numero,
                                   piso_puerta = @pisoPuerta,
                                   codigo_postal = @codigoPostal,
                                   localidad = @localidad,
                                   provincia = @provincia
                               WHERE id_cliente = @id";

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    CargarParametros(cmd, cliente);
                    cmd.Parameters.AddWithValue("@id", cliente.IdCliente);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Desactivar(int idCliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                // Iniciamos una transacción para asegurar la integridad de los datos
                using (MySqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Desactivar al Cliente
                        string sqlCliente = "UPDATE clientes SET activo = FALSE WHERE id_cliente = @id";
                        using (MySqlCommand cmdCliente = new MySqlCommand(sqlCliente, con, trans))
                        {
                            cmdCliente.Parameters.AddWithValue("@id", idCliente);
                            cmdCliente.ExecuteNonQuery();
                        }

                        // 2. Desactivar todas sus Mascotas
                        string sqlMascotas = "UPDATE mascotas SET activo = FALSE WHERE id_cliente = @id";
                        using (MySqlCommand cmdMascotas = new MySqlCommand(sqlMascotas, con, trans))
                        {
                            cmdMascotas.Parameters.AddWithValue("@id", idCliente);
                            cmdMascotas.ExecuteNonQuery();
                        }

                        // Si todo ha ido bien, confirmamos los cambios
                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        // Si algo falla, deshacemos cualquier cambio pendiente
                        trans.Rollback();
                        throw; // Re-lanzamos la excepción para que el Service/UI la capture
                    }
                }
            }
        }

        public bool Reactivar(int idCliente)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = "UPDATE clientes SET activo = TRUE WHERE id_cliente = @id";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", idCliente);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public int ContarClientes()
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM clientes";
                MySqlCommand cmd = new MySqlCommand(sql, con);
                return Convert.ToInt32(cmd.ExecuteScalar());
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
                Activo = Convert.ToBoolean(rdr["activo"]),
                // Mapeo de nuevos campos de dirección
                CalleDireccion = rdr["calle"].ToString(),
                NumeroDireccion = rdr["numero"].ToString(),
                PisoPuertaDireccion = rdr["piso_puerta"].ToString(),
                CodigoPostalDireccion = rdr["codigo_postal"].ToString(),
                LocalidadDireccion = rdr["localidad"].ToString(),
                ProvinciaDireccion = rdr["provincia"].ToString(),
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
            cmd.Parameters.AddWithValue("@calle", cliente.CalleDireccion);
            cmd.Parameters.AddWithValue("@numero", cliente.NumeroDireccion);
            cmd.Parameters.AddWithValue("@pisoPuerta", cliente.PisoPuertaDireccion);
            cmd.Parameters.AddWithValue("@codigoPostal", cliente.CodigoPostalDireccion);
            cmd.Parameters.AddWithValue("@localidad", cliente.LocalidadDireccion);
            cmd.Parameters.AddWithValue("@provincia", cliente.ProvinciaDireccion);
            cmd.Parameters.AddWithValue("@fechaAlta", cliente.FechaAlta);
        }
    }
}