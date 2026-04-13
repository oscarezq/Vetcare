using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que almacena el usuario con el que está iniciada la sesión.
    /// </summary>
    public static class Sesion
    {
        /// <summary>
        /// Usuario con el que está abierta la sesión en la aplicación.
        /// </summary>
        public static Usuario? UsuarioActual { get; set; }
    }
}
