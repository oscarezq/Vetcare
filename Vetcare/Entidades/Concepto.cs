using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un concepto (producto o servicio) registrado en el sistema.
    /// </summary>
    public class Concepto
    {
        /// <summary>
        /// Identificador del concepto.
        /// </summary>
        public int IdConcepto { get; set; }

        /// <summary>
        /// Tipo de concepto ('Producto', 'Servicio').
        /// </summary>
        public string? Tipo { get; set; }

        /// <summary>
        /// Nombre del concepto.
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Descripción del concepto.
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Precio (con IVA) del concepto.
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Porcentaje de IVA que se aplica al concepto ('4', '10', '21').
        /// </summary>
        public decimal IvaPorcentaje { get; set; }

        /// <summary>
        /// Indica si el concepto está activo en el sistema.
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Stock disponible (solo para los productos).
        /// </summary>
        public int? Stock { get; set; }

        /// <summary>
        /// Fecha de alta del concepto en el sistema.
        /// </summary>
        public DateTime FechaAlta { get; set; }
    }
}
