using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un rol de usuario registrado en el sistema.
    /// </summary>
    public class Rol
    {
        /// <summary>
        /// Identificador del rol.
        /// </summary>
        public int IdRol { get; set; }

        /// <summary>
        /// Nombre del rol ('Administrador', 'Veterinario', 'Recepcionista').
        /// </summary>
        public string? NombreRol { get; set; }

        /// <summary>
        /// Descripción del rol.
        /// </summary>
        public string? Descripcion { get; set; }
    }
}
