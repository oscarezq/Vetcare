using System;
using System.Security.Cryptography;
using System.Text;

namespace Vetcare.Utilidades
{
    public class Seguridad
    {
        // Genera un salt aleatorio
        public static string GenerarSalt()
        {
            byte[] saltBytes = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        // Genera el hash usando password + salt
        public static string Encriptar(string password, string salt)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password + salt)
                );

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
    }
}
