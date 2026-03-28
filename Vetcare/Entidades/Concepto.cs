using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    public class Concepto
    {
        public int IdConcepto { get; set; }
        public string Tipo { get; set; } // Producto o Servicio
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal IvaPorcentaje { get; set; }

        public decimal PrecioSinIva
        {
            get
            {
                return Precio / (1 + (IvaPorcentaje / 100m));
            }
        }

        public bool Activo { get; set; }
        public int? Stock { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
