using System;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Lógica de negocio para la gestión de usuarios.
    /// </summary>
    public class UsuarioService
    {
        // Instanciamos el DAO para comunicarnos con la base de datos
        private UsuarioDAO _usuarioDAO = new UsuarioDAO();

        /// <summary>
        /// Realiza las validaciones de negocio para el inicio de sesión.
        /// </summary>
        /// <param name="user">Username introducido.</param>
        /// <param name="pass">Password introducida.</param>
        /// <param name="usuarioLogueado">Parámetro de salida que contendrá el objeto Usuario si el login es correcto.</param>
        /// <returns>Un mensaje de error detallado o una cadena vacía si el login es exitoso.</returns>
        public string ValidarLogin(string user, string pass, out Usuario usuarioLogueado)
        {
            usuarioLogueado = null;
            string mensaje = string.Empty;

            // Validaciones previas
            if (string.IsNullOrWhiteSpace(user))
                return "El nombre de usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(pass))
                return "La contraseña es obligatoria.";

            try
            {
                // Llamada al DAO para verificar credenciales
                usuarioLogueado = _usuarioDAO.Login(user, pass);

                // Si el DAO devuelve null, es que el usuario no existe, la clave está mal o está inactivo
                if (usuarioLogueado == null)
                {
                    mensaje = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                // Capturamos errores de conexión o SQL
                mensaje = "Error de conexión: " + ex.Message;
            }

            return mensaje;
        }
    }
}
