using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa los detalles de una factura registrada en el sistema.
    /// </summary>
    public class DetalleFactura
    {
        /// <summary>
        /// Identificador del detalle de factura.
        /// </summary>
        public int IdDetalle { get; set; }

        /// <summary>
        /// Identificador de la factura asociada al detalle de factura.
        /// </summary>
        public int IdFactura { get; set; }

        /// <summary>
        /// Identificador del concepto asociado al detalle de factura.
        /// </summary>
        public int IdConcepto { get; set; }

        /// <summary>
        /// Nombre del concepto asociado al detalle de factura.
        /// </summary>
        public string? NombreConcepto { get; set; }

        /// <summary>
        /// Tipo ('Producto', 'Servicio') del concepto asociado al detalle de factura.
        /// </summary>
        public string? Tipo { get; set; }

        /// <summary>
        /// Cantidad del concepto asociado al detalle de factura.
        /// </summary>
        public int Cantidad { get; set; } = 1;

        /// <summary>
        /// Precio unitario (con IVA) del concepto asociado al detalle de factura.
        /// </summary>
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Porcentaje de IVA aplicado al concepto asociado al detalle de factura.
        /// </summary>
        public decimal IvaPorcentaje { get; set; }

        /// <summary>
        /// Precio unitario (sin IVA) del concepto asociado al detalle de factura.
        /// </summary>
        public decimal PrecioSinIva
        {
            get => PrecioUnitario / (1 + (IvaPorcentaje / 100m));
            set => PrecioUnitario = value * (1 + (IvaPorcentaje / 100m));
        }

        /// <summary>
        /// Subtotal de la línea (cantidad por precio sin IVA).
        /// </summary>
        public decimal Subtotal
        {
            get => Cantidad * PrecioSinIva;
            set { }
        }

        /// <summary>
        /// Importe de IVA correspondiente a la línea.
        /// </summary>
        public decimal IvaImporte
        {
            get => Subtotal * (IvaPorcentaje / 100m);
            set { }
        }

        /// <summary>
        /// Total de la línea (cantidad por precio unitario con IVA).
        /// </summary>
        public decimal TotalLinea
        {
            get => Cantidad * PrecioUnitario;
            set { }
        }
    }
}