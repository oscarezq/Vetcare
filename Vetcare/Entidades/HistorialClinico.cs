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
        public int IdServicio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Propiedades extendidas para la interfaz (DataGrid)
        public string NombreServicio { get; set; }

        // Propiedad calculada: No está en la BD pero es vital para el DataGrid
        public decimal Subtotal => Cantidad * PrecioUnitario;

        public DetalleFactura()
        {
            Cantidad = 1;
        }
    }
}
