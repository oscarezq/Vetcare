using System;
using MySql.Data.MySqlClient;

namespace Vetcare.Datos
{
    /// <summary>
    /// Clase encargada de gestionar la conexión con la base de datos MySQL.
    /// </summary>
    public class Conexion
    {
        /// <summary>
        /// Cadena de conexión que contiene los parámetros del servidor, base de datos y credenciales.
        /// </summary>
        private string cadena = "Server=localhost; Database=vetcare; Uid=root; Pwd=;";

        /// <summary>
        /// Crea y devuelve un objeto de conexión a la base de datos.
        /// </summary>
        /// <returns>Un objeto <see cref="MySqlConnection"/> configurado con la cadena de conexión.</returns>
        public MySqlConnection ObtenerConexion()
        {
            // Creamos el objeto de conexión con la cadena definida arriba
            MySqlConnection conectar = new MySqlConnection(cadena);
            return conectar;
        }
    }
}