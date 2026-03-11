using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioBase { get; set; }

        // Sobrescribir ToString para que aparezca el nombre en ComboBoxes si es necesario
        public override string ToString() => Nombre;
    }
}
