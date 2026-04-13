using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa una especie registrada en el sistema.
    /// </summary>
    public class Especie
    {
        /// <summary>
        /// Identificador de la especie.
        /// </summary>
        public int IdEspecie { get; set; }

        /// <summary>
        /// Nombre de la especie.
        /// </summary>
        public string? NombreEspecie { get; set; }
    }
}
