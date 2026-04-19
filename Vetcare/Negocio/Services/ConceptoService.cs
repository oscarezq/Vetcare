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
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los conceptos (servicios y productos). 
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (ConceptoDAO).
    /// </summary>
    public class ConceptoService
    {
        /// <summary>
        /// Instancia de acceso a datos para los conceptos.
        /// </summary>
        private ConceptoDAO servicioDAO = new ConceptoDAO();

        /// <summary>
        /// Obtiene todos los conceptos registrados.
        /// </summary>
        /// <returns>Lista de conceptos.</returns>
        public List<Concepto> ObtenerTodos()
        {
            return servicioDAO.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un concepto por su identificador.
        /// </summary>
        /// <param name="idConcepto">ID del concepto.</param>
        /// <returns>El concepto encontrado.</returns>
        public Concepto ObtenerPorId(int idConcepto)
        {
            return servicioDAO.ObtenerPorId(idConcepto);
        }

        /// <summary>
        /// Obtiene todos los conceptos de tipo servicio.
        /// </summary>
        /// <returns>Lista de servicios.</returns>
        public List<Concepto> ObtenerServicios()
        {
            return servicioDAO.ObtenerServicios();
        }

        /// <summary>
        /// Obtiene todos los conceptos de tipo producto.
        /// </summary>
        /// <returns>Lista de productos.</returns>
        public List<Concepto> ObtenerProductos()
        {
            return servicioDAO.ObtenerProductos();
        }

        /// <summary>
        /// Inserta un nuevo concepto (servicio o producto).
        /// </summary>
        /// <param name="servicio">Objeto concepto a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Concepto servicio)
        {
            return servicioDAO.Insertar(servicio);
        }

        /// <summary>
        /// Actualiza un concepto existente.
        /// </summary>
        /// <param name="servicio">Objeto concepto con los datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Concepto servicio)
        {
            return servicioDAO.Actualizar(servicio);
        }

        /// <summary>
        /// Elimina un concepto (baja lógica o física según implementación).
        /// </summary>
        /// <param name="idConcepto">ID del concepto.</param>
        /// <returns>True si se elimina correctamente, false en caso contrario.</returns>
        public bool Eliminar(int idConcepto)
        {
            return servicioDAO.Eliminar(idConcepto);
        }

        /// <summary>
        /// Reactiva un concepto previamente eliminado o desactivado.
        /// </summary>
        /// <param name="idConcepto">ID del concepto.</param>
        /// <returns>True si se reactiva correctamente, false en caso contrario.</returns>
        public bool Reactivar(int idConcepto)
        {
            return servicioDAO.Reactivar(idConcepto);
        }
    }
}