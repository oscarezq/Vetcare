using System;
using System.Security.Cryptography;
using System.Text;

namespace Vetcare.Utilidades
{
    /// <summary>
    /// Clase encargada de gestionar la seguridad de contraseñas mediante generación de salt y hash.
    /// </summary>
    public class Seguridad
    {
        /// <summary>
        /// Genera un salt aleatorio para aumentar la seguridad de las contraseñas.
        /// </summary>
        /// <returns>Un string en Base64 que representa el salt generado.</returns>
        public static string GenerarSalt()
        {
            // Creamos un array de bytes de 16 posiciones para el salt
            byte[] saltBytes = new byte[16];

            // Usamos un generador criptográfico seguro
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            // Convertimos el array de bytes a string en formato Base64
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Genera un hash SHA256 combinando la contraseña con el salt.
        /// </summary>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <param name="salt">Salt previamente generado.</param>
        /// <returns>Hash de la contraseña en formato hexadecimal.</returns>
        public static string Encriptar(string password, string salt)
        {
            // Convertimos la contraseña + salt a bytes y generamos el hash
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)
);
            // Convertimos el hash a formato hexadecimal
            StringBuilder builder = new();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));

            // Devolvemos el hash como string
            return builder.ToString();
        }
    }
}