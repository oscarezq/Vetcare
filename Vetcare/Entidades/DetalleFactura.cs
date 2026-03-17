using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    public class DetalleFactura
    {
        public int IdDetalle { get; set; }
        public int IdFactura { get; set; }
        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IvaPorcentaje { get; set; }
        public decimal Subtotal
        {
            get { return Cantidad * PrecioUnitario; }
        }
        public DetalleFactura()
        {
            Cantidad = 1;
        }
    }
}
