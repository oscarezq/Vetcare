using System;

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

        // ESTE ES CON IVA (lo que guardas en BD)
        public decimal PrecioUnitario { get; set; }

        public decimal IvaPorcentaje { get; set; }

        // 🔹 Precio sin IVA (calculado)
        public decimal PrecioSinIva
        {
            get => PrecioUnitario / (1 + (IvaPorcentaje / 100m));
            set => PrecioUnitario = value * (1 + (IvaPorcentaje / 100m)); // opcional pero útil
        }

        // 🔹 Base imponible
        public decimal Subtotal
        {
            get => Cantidad * PrecioSinIva;
            set { } // necesario para mapeo BD (aunque no lo uses)
        }

        // 🔹 IVA €
        public decimal IvaImporte
        {
            get => Subtotal * (IvaPorcentaje / 100m);
            set { }
        }

        // 🔹 Total línea (con IVA)
        public decimal TotalLinea
        {
            get => Cantidad * PrecioUnitario;
            set { }
        }

        public DetalleFactura()
        {
            Cantidad = 1;
        }
    }
}