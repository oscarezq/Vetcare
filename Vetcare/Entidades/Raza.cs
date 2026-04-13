using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa una raza registrada en el sistema.
    /// </summary>
    public class Raza
    {
        /// <summary>
        /// Identificador de la raza.
        /// </summary>
        public int IdRaza { get; set; }

        /// <summary>
        /// Identificar de la especie a la que pertenece la raza.
        /// </summary>
        public int IdEspecie { get; set; }

        /// <summary>
        /// Nombre de la raza.
        /// </summary>
        public string? NombreRaza { get; set; }

        /// <summary>
        /// Nombre de la especie a la que pertenece la raza.
        /// </summary>
        public string? NombreEspecie { get; set; }
    }
}
