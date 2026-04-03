using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vetcare.Entidades
{
    public class Factura
    {
        public int IdFactura { get; set; }
        public int IdCliente { get; set; }
        public string NumeroFactura { get; set; }
        public string Estado { get; set; } // "Pendiente", "Pagada", "Anulada"
        public DateTime FechaEmision { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal IvaTotal { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; } 
        public string Observaciones { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidosCliente { get; set; }
        public string NombreApellidosCliente => $"{NombreCliente} {ApellidosCliente}".Trim();
        public string NumeroDocumentoCliente { get; set; }
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();

        public Factura()
        {
            FechaEmision = DateTime.Now;
        }
    }
}
