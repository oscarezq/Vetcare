using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada de realizar las operaciones de acceso a datos relacionadas con 
    /// la entidad Mascota en la base de datos vetcare.
    /// </summary>
    class MascotaDAO
    {
        // Objeto para obtener la conexión
        Conexion conexion = new Conexion();

        /// <summary>
        /// Método para obtener una lista con todas las mascotas que hay en la tabla mascotas 
        /// de la base de datos vetcare.
        /// </summary>
        /// <returns>Lista con todas las mascotas.</returns>
        public List<Mascota> ObtenerTodas()
        {
            // Lista con todas las mascotas
            List<Mascota> lista = new List<Mascota>();

            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para obtener todas las mascotas
                string sql = @"SELECT m.id_mascota,
                                    m.id_cliente,
                                    m.numero_chip,
                                    m.nombre,
                                    m.especie,
                                    m.raza,
                                    m.sexo,
                                    m.peso,
                                    m.fecha_nacimiento,
                                    m.id_cliente,
                                    c.nombre AS nombre_dueno,
                                    c.apellidos AS apellidos_dueno,
                                    c.num_documento AS documento_dueno
                             FROM Mascotas m
                             INNER JOIN Clientes c ON m.id_cliente = c.id_cliente";

                // Generamos el comando
                MySqlCommand cmd = new MySqlCommand(sql, con);

                // Ejecutamos la consulta con ExecuteReader para obtener un MySqlDataReader
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    // Mientras haya registros en el MySqlDataReader, los leemos
                    while (rdr.Read())
                    {
                        // Creamos una nueva mascota con los datos leidos
                        Mascota mascota = new Mascota
                        {
                            IdMascota = Convert.ToInt32(rdr["id_mascota"]),
                            IdCliente = Convert.ToInt32(rdr["id_cliente"]),
                            NumeroChip = rdr["numero_chip"].ToString(),
                            Nombre = rdr["nombre"].ToString(),
                            Especie = rdr["especie"].ToString(),
                            Raza = rdr["raza"].ToString(),
                            Sexo = rdr["sexo"].ToString(),
                            Peso = Convert.ToDecimal(rdr["peso"]),
                            FechaNacimiento = Convert.ToDateTime(rdr["fecha_nacimiento"]),
                            NombreDueno = rdr["nombre_dueno"].ToString(),
                            ApellidosDueno = rdr["apellidos_dueno"].ToString(),
                            NumeroIdentificacionDueno = rdr["documento_dueno"].ToString(),
                        };

                        // Añadimos a la lista la mascota recien creada
                        lista.Add(mascota);
                    }
                }
            }

            // Devolvemos la lista
            return lista;
        }

        /// <summary>
        /// Obtiene una mascota concreta de la base de datos según su ID.
        /// </summary>
        /// <param name="idMascota">ID de la mascota a buscar</param>
        /// <returns>
        /// Devuelve un objeto Mascota si existe.
        /// Devuelve null si no se encuentra.
        /// </returns>
        public Mascota ObtenerPorId(int idMascota)
        {
            Mascota mascota = null;

            try
            {
                // Obtenemos la conexión
                using (MySqlConnection con = conexion.ObtenerConexion())
                {
                    // Abrimos la conexión
                    con.Open();

                    // Consulta SQL para obtener una mascota concreta
                    string sql = @"SELECT m.id_mascota,
                                    m.numero_chip,
                                    m.nombre,
                                    m.especie,
                                    m.raza,
                                    m.sexo,
                                    m.peso,
                                    m.fecha_nacimiento,
                                    m.id_cliente,
                                    c.nombre AS nombre_dueno,
                                    c.apellidos AS apellidos_dueno,
                                    c.num_documento AS documento_dueno
                             FROM Mascotas m
                             INNER JOIN Clientes c ON m.id_cliente = c.id_cliente
                             WHERE m.id_mascota = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        // Parámetro para evitar inyección SQL
                        cmd.Parameters.AddWithValue("@id", idMascota);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Si encuentra resultado
                            if (reader.Read())
                            {
                                mascota = new Mascota
                                {
                                    IdMascota = Convert.ToInt32(reader["id_mascota"]),
                                    NumeroChip = reader["numero_chip"].ToString(),
                                    Nombre = reader["nombre"].ToString(),
                                    Especie = reader["especie"].ToString(),
                                    Raza = reader["raza"].ToString(),
                                    Sexo = reader["sexo"].ToString(),
                                    Peso = Convert.ToDecimal(reader["peso"]),
                                    FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"]),
                                    IdCliente = Convert.ToInt32(reader["id_cliente"]),
                                    NombreDueno = reader["nombre_dueno"].ToString(),
                                    ApellidosDueno = reader["apellidos_dueno"].ToString(),
                                    NumeroIdentificacionDueno = reader["documento_dueno"].ToString(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener mascota por ID: " + ex.Message);
            }

            return mascota;
        }

        /// <summary>
        /// Método para obtener todas las mascotas pertenecientes a un cliente
        /// de la tabla mascotas de la base de datos vetcare.
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Lista de mascotas del cliente indicado</returns>
        public List<Mascota> ObtenerPorCliente(int idCliente)
        {
            // Creamos la lista donde guardaremos las mascotas
            List<Mascota> lista = new List<Mascota>();

            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Consulta para obtener las mascotas del cliente
                string sql = @"SELECT m.id_mascota,
                                    m.numero_chip,
                                    m.nombre,
                                    m.especie,
                                    m.raza,
                                    m.sexo,
                                    m.peso,
                                    m.fecha_nacimiento,
                                    m.id_cliente,
                                    c.nombre AS nombre_dueno,
                                    c.apellidos AS apellidos_dueno,
                                    c.num_documento AS documento_dueno
                             FROM Mascotas m
                             INNER JOIN Clientes c ON m.id_cliente = c.id_cliente
                             WHERE m.id_cliente = @idCliente";

                // Creamos el comando
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idCliente", idCliente);

                // Abrimos la conexión
                con.Open();

                // Ejecutamos la consulta
                MySqlDataReader reader = cmd.ExecuteReader();

                // Recorremos los resultados
                while (reader.Read())
                {
                    // Creamos un objeto Mascota
                    Mascota mascota = new Mascota
                    {
                        IdMascota = Convert.ToInt32(reader["id_mascota"]),
                        IdCliente = Convert.ToInt32(reader["id_cliente"]),
                        NumeroChip = reader["numero_chip"].ToString(),
                        Nombre = reader["nombre"].ToString(),
                        Especie = reader["especie"].ToString(),
                        Raza = reader["raza"].ToString(),
                        Sexo = reader["sexo"].ToString(),
                        Peso = Convert.ToDecimal(reader["peso"]),
                        FechaNacimiento = Convert.ToDateTime(reader["fecha_nacimiento"])
                    };

                    // Añadimos la mascota a la lista
                    lista.Add(mascota);
                }
            }

            // Devolvemos la lista
            return lista;
        }

        /// <summary>
        /// Método para insertar una nueva mascota en la tabla mascotas 
        /// de la base de datos vetcare.
        /// </summary>
        /// <param name="mascota">Mascota que se va a insertar.</param>
        /// <returns>True si se ha insertado correctamente; false en caso contrario.</returns>
        public bool Insertar(Mascota mascota)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para insertar mascota
                string sql = @"INSERT INTO mascotas 
                               (id_cliente, numero_chip, nombre, especie, raza, sexo, peso, fecha_nacimiento)
                               VALUES (@idCliente, @numeroChip, @nombre, @especie, @raza, @sexo, @peso, @fechaNacimiento)";

                // Creamos el comando y le añadimos los parámetros del cliente
                MySqlCommand cmd = new MySqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@idCliente", mascota.IdCliente);
                cmd.Parameters.AddWithValue("@numeroChip", mascota.NumeroChip);
                cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                cmd.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);

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
        /// Método para insertar varias mascotas en la tabla mascotas 
        /// de la base de datos vetcare.
        /// </summary>
        /// <param name="mascotas">Lista de mascotas que se van a insertar.</param>
        /// <returns>True si todas se insertaron correctamente.</returns>
        public bool InsertarVarios(List<Mascota> mascotas)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión e iniciamos una transacción
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Consulta para insertar mascota
                    string sql = @"INSERT INTO mascotas 
                           (id_cliente, numero_chip, nombre, especie, raza, sexo, peso, fecha_nacimiento)
                           VALUES (@idCliente, @numeroChip, @nombre, @especie, @raza, @sexo, @peso, @fechaNacimiento)";

                    // Recorremos la lista de mascotas
                    foreach (Mascota mascota in mascotas)
                    {
                        // Creamos el comando y le añadimos los parámetros del cliente
                        MySqlCommand cmd = new MySqlCommand(sql, con, transaccion);

                        cmd.Parameters.AddWithValue("@idCliente", mascota.IdCliente);
                        cmd.Parameters.AddWithValue("@numeroChip", mascota.NumeroChip);
                        cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                        cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                        cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                        cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                        cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                        cmd.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);

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
        /// Método para eliminar una mascota de la tabla mascotas 
        /// de la base de datos vetcare.
        /// </summary>
        /// <param name="idMascota">Identificador de la mascota a eliminar.</param>
        /// <returns>True si se eliminó correctamente; false en caso contrario.</returns>
        public bool Eliminar(int idMascota)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para eliminar mascotas con ese identificador
                string query = "DELETE FROM mascotas WHERE id_mascota = @id";

                // Cremos el comando y le añadimos el parámetro
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idMascota);

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
        /// Método para eliminar varias mascotas de la tabla mascotas 
        /// de la base de datos vetcare.
        /// </summary>
        /// <param name="idsMascotas">Lista de identificadores de las mascotas a eliminar.</param>
        /// <returns>True si todas se eliminaron correctamente.</returns>
        public bool EliminarVarios(List<int> idsMascotas)
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
                    foreach (int id in idsMascotas)
                    {
                        // Consulta para eliminar la mascota con ese identificador
                        string query = "DELETE FROM mascotas WHERE id_mascota = @id";

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
                    // Si algo falla hacemos rollback
                    transaccion.Rollback();
                    return false;
                }
            }

            // Si todo ha ido bien devolvemos true
            return true;
        }

        /// <summary>
        /// Método para actualizar los datos de una mascota existente 
        /// en la tabla mascotas de la base de datos vetcare.
        /// </summary>
        /// <param name="mascota">Objeto mascota con los nuevos datos.</param>
        /// <returns>True si la actualización fue correcta; false en caso contrario.</returns>
        public bool Actualizar(Mascota mascota)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión
                con.Open();

                // Consulta para actualizar mascota
                string query = @"UPDATE mascotas 
                                 SET id_cliente = @idCliente,
                                     numero_chip = @numeroChip,
                                     nombre = @nombre,
                                     especie = @especie,
                                     raza = @raza,
                                     sexo = @sexo,
                                     peso = @peso,
                                     fecha_nacimiento = @fechaNacimiento
                                 WHERE id_mascota = @id";

                // Cremos el comando y le añadimos el parámetro
                using (MySqlCommand comando = new MySqlCommand(query, con))
                {
                    comando.Parameters.AddWithValue("@idCliente", mascota.IdCliente);
                    comando.Parameters.AddWithValue("@numeroChip", mascota.NumeroChip);
                    comando.Parameters.AddWithValue("@nombre", mascota.Nombre);
                    comando.Parameters.AddWithValue("@especie", mascota.Especie);
                    comando.Parameters.AddWithValue("@raza", mascota.Raza);
                    comando.Parameters.AddWithValue("@sexo", mascota.Sexo);
                    comando.Parameters.AddWithValue("@peso", mascota.Peso);
                    comando.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);
                    comando.Parameters.AddWithValue("@id", mascota.IdMascota);

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
        /// Método para actualizar múltiples mascotas en una sola transacción.
        /// Si ocurre un error, se revierte toda la operación.
        /// </summary>
        /// <param name="mascotas">Lista de mascotas a actualizar.</param>
        /// <returns>True si todas se actualizaron correctamente.</returns>
        public bool ActualizarVarios(List<Mascota> mascotas)
        {
            // Obtenemos la conexión
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                // Abrimos la conexión e iniciamos una transacción
                con.Open();
                MySqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Recorremos todas las mascotas de la lista de mascotas
                    foreach (Mascota mascota in mascotas)
                    {
                        // Consulta para actualizar mascotas
                        string query = @"UPDATE mascotas 
                                         SET id_cliente = @idCliente,
                                             numero_chip = @numeroChip,
                                             nombre = @nombre,
                                             especie = @especie,
                                             raza = @raza,
                                             sexo = @sexo,
                                             peso = @peso,
                                             fecha_nacimiento = @fechaNacimiento
                                         WHERE id_mascota = @id";

                        // Cremos el comando y le añadimos el parámetro
                        using (MySqlCommand comando = new MySqlCommand(query, con, transaccion))
                        {
                            comando.Parameters.AddWithValue("@idCliente", mascota.IdCliente);
                            comando.Parameters.AddWithValue("@numeroChip", mascota.NumeroChip);
                            comando.Parameters.AddWithValue("@nombre", mascota.Nombre);
                            comando.Parameters.AddWithValue("@especie", mascota.Especie);
                            comando.Parameters.AddWithValue("@raza", mascota.Raza);
                            comando.Parameters.AddWithValue("@sexo", mascota.Sexo);
                            comando.Parameters.AddWithValue("@peso", mascota.Peso);
                            comando.Parameters.AddWithValue("@fechaNacimiento", mascota.FechaNacimiento);
                            comando.Parameters.AddWithValue("@id", mascota.IdMascota);

                            // Ejecutamos el comando para actualizar la mascota
                            comando.ExecuteNonQuery();
                        }
                    }

                    // Si se han actualizado todas las mascotas, hacemos un commit
                    transaccion.Commit();
                }
                catch
                {
                    // Si algo ha fallado, hacemos un rollback para que no se actualice ninguna mascota
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