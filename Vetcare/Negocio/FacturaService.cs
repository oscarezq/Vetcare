using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    public class FacturaService
    {
        private FacturaDAO facturaDAO = new FacturaDAO();

        /// <summary>
        /// Crea una factura completa con sus líneas de detalle en una sola operación.
        /// </summary>
        public bool InsertarFacturaCompleta(Factura factura)
        {
            return facturaDAO.InsertarFacturaCompleta(factura);
        }

        public List<Factura> ObtenerTodas()
        {
            return facturaDAO.ObtenerTodas();
        }

        public List<Factura> ObtenerPorCliente(int idCliente)
        {
            return facturaDAO.ObtenerPorCliente(idCliente);
        }

        public decimal CalcularDeudaCliente(int idCliente)
        {
            return facturaDAO.CalcularDeudaCliente(idCliente);
        }

        public List<Factura> ObtenerFacturasPendientes()
        {
            return facturaDAO.ObtenerFacturasPendientes();
        }

        public bool AnularFactura(int idFactura)
        {
            return facturaDAO.AnularFactura(idFactura);
        }

        public string ObtenerUltimoNumeroPorAnio(int anioActual)
        {
            return facturaDAO.ObtenerUltimoNumeroPorAnio(anioActual);
        }

        public bool ActualizarEstadoFactura(int idFactura, string nuevoEstado)
        {
            return facturaDAO.ActualizarEstadoFactura(idFactura, nuevoEstado);
        }

        public decimal ObtenerIngresosMes()
        {
            return facturaDAO.ObtenerIngresosMes();
        }
    }
}
