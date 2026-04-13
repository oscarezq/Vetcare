using System;
using System.Collections.Generic;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa una factura en el sistema.
    /// </summary>
    public class Factura
    {
        /// <summary>
        /// Identificador de la factura.
        /// </summary>
        public int IdFactura { get; set; }

        /// <summary>
        /// Identificador del cliente asociado a la factura.
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Número de factura.
        /// </summary>
        public string? NumeroFactura { get; set; }

        /// <summary>
        /// Estado de la factura ('Pendiente', 'Pagada', 'Anulada').
        /// </summary>
        public string? Estado { get; set; }

        /// <summary>
        /// Fecha de emisión de la factura.
        /// </summary>
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        /// <summary>
        /// Base imponible (precio total sin IVA).
        /// </summary>
        public decimal BaseImponible { get; set; }

        /// <summary>
        /// Cantidad de dinero a pagar por concepto de IVA.
        /// </summary>
        public decimal IvaTotal { get; set; }

        /// <summary>
        /// Precio final a pagar (base imponible + IVA).
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Método de pago que va a usar el cliente para pagar la factura ('Efectivo', 'Transferencia', 'Tarjeta').
        /// </summary>
        public string? MetodoPago { get; set; } 

        /// <summary>
        /// Observaciones para la factura.
        /// </summary>
        public string? Observaciones { get; set; }

        /// <summary>
        /// Nombre del cliente asociado a la factura.
        /// </summary>
        public string? NombreCliente { get; set; }

        /// <summary>
        /// Apellidos del cliente asociado a la factura.
        /// </summary>
        public string? ApellidosCliente { get; set; }

        /// <summary>
        /// Nombre + Apellidos del cliente asociado a la factura.
        /// </summary>
        public string? NombreApellidosCliente => $"{NombreCliente} {ApellidosCliente}".Trim();

        /// <summary>
        /// Número de documento de identifación del cliente asociado a la factura.
        /// </summary>
        public string? NumeroDocumentoCliente { get; set; }

        /// <summary>
        /// Lista con todos los detalles asociados a la factura.
        /// </summary>
        public List<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}
