using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class DetalleFacturaService
    {
        private DetalleFacturaDAO DetalleFacturaDAO = new();

        public List<DetalleFactura> ObtenerDetallesPorFactura(int idFactura)
        {
            return DetalleFacturaDAO.ObtenerDetallesPorFactura(idFactura);
        }
    }
}