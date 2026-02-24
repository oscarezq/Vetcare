using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un usuario registrado en el sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador del usuario.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Identificador del rol del usuario.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Username del usuario.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contraseña del usuario (encriptada).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellidos del usuario.
        /// </summary>
        public string Apellidos { get; set; }

        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Teléfono del usuario.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Booleano que indica si el usuario está activo o no.
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Fecha de alta del usuario.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Nombre del rol del usuario.
        /// </summary>
        public string NombreRol { get; set; }
    }
}