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
    /// Servicio encargado de gestionar la lógica de negocio relacionada con las facturas.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (FacturaDAO).
    /// </summary>
    public class FacturaService
    {
        /// <summary>
        /// Instancia de acceso a datos para las facturas.
        /// </summary>
        private readonly FacturaDAO facturaDAO = new();

        /// <summary>
        /// Inserta una factura completa junto con sus detalles asociados.
        /// </summary>
        /// <param name="factura">Objeto factura a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool InsertarFacturaCompleta(Factura factura)
        {
            return facturaDAO.InsertarFacturaCompleta(factura);
        }

        /// <summary>
        /// Obtiene todas las facturas registradas.
        /// </summary>
        /// <returns>Lista de facturas.</returns>
        public List<Factura> ObtenerTodas()
        {
            return facturaDAO.ObtenerTodas();
        }

        /// <summary>
        /// Obtiene las facturas asociadas a un cliente específico.
        /// </summary>
        /// <param name="idCliente">ID del cliente.</param>
        /// <returns>Lista de facturas del cliente.</returns>
        public List<Factura> ObtenerPorCliente(int idCliente)
        {
            return facturaDAO.ObtenerPorCliente(idCliente);
        }

        /// <summary>
        /// Calcula la deuda total pendiente de un cliente.
        /// </summary>
        /// <param name="idCliente">ID del cliente.</param>
        /// <returns>Importe total de la deuda.</returns>
        public decimal CalcularDeudaCliente(int idCliente)
        {
            return facturaDAO.CalcularDeudaCliente(idCliente);
        }

        /// <summary>
        /// Obtiene las facturas que están pendientes de pago.
        /// </summary>
        /// <returns>Lista de facturas pendientes.</returns>
        public List<Factura> ObtenerFacturasPendientes()
        {
            return facturaDAO.ObtenerFacturasPendientes();
        }

        /// <summary>
        /// Anula una factura existente.
        /// </summary>
        /// <param name="idFactura">ID de la factura.</param>
        /// <returns>True si se anula correctamente, false en caso contrario.</returns>
        public bool AnularFactura(int idFactura)
        {
            return facturaDAO.AnularFactura(idFactura);
        }

        /// <summary>
        /// Obtiene el último número de factura generado en un año específico.
        /// </summary>
        /// <param name="anioActual">Año actual.</param>
        /// <returns>Último número de factura o null si no existe.</returns>
        public string? ObtenerUltimoNumeroPorAnio(int anioActual)
        {
            return facturaDAO.ObtenerUltimoNumeroPorAnio(anioActual);
        }

        /// <summary>
        /// Actualiza el estado de una factura.
        /// </summary>
        /// <param name="idFactura">ID de la factura.</param>
        /// <param name="nuevoEstado">Nuevo estado de la factura.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool ActualizarEstadoFactura(int idFactura, string nuevoEstado)
        {
            return facturaDAO.ActualizarEstadoFactura(idFactura, nuevoEstado);
        }

        /// <summary>
        /// Obtiene los ingresos totales del mes actual.
        /// </summary>
        /// <returns>Importe total de ingresos del mes.</returns>
        public decimal ObtenerIngresosMes()
        {
            return facturaDAO.ObtenerIngresosMes();
        }
    }
}