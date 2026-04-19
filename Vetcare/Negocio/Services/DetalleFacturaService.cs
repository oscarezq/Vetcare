using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los detalles de factura.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (DetalleFacturaDAO).
    /// </summary>
    class DetalleFacturaService
    {
        /// <summary>
        /// Instancia de acceso a datos para los detalles de factura.
        /// </summary>
        private DetalleFacturaDAO DetalleFacturaDAO = new();

        /// <summary>
        /// Obtiene los detalles asociados a una factura específica.
        /// </summary>
        /// <param name="idFactura">ID de la factura.</param>
        /// <returns>Lista de detalles de la factura.</returns>
        public List<DetalleFactura> ObtenerDetallesPorFactura(int idFactura)
        {
            return DetalleFacturaDAO.ObtenerDetallesPorFactura(idFactura);
        }
    }
}