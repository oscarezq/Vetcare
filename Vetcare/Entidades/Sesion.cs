using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    public static class Sesion
    {
        public static Usuario UsuarioActual { get; set; }

        public static void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}
