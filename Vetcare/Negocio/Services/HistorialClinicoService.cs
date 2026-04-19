using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con el historial clínico.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (HistorialClinicoDAO).
    /// </summary>
    public class HistorialClinicoService
    {
        /// <summary>
        /// Instancia de acceso a datos para el historial clínico.
        /// </summary>
        private readonly HistorialClinicoDAO historialClinicoDAO = new();

        /// <summary>
        /// Obtiene un historial clínico asociado a una cita.
        /// </summary>
        /// <param name="id">ID de la cita.</param>
        /// <returns>Historial clínico o null si no existe.</returns>
        public HistorialClinico? ObtenerPorIdCita(int id)
        {
            return historialClinicoDAO.ObtenerPorIdCita(id);
        }

        /// <summary>
        /// Obtiene el historial clínico de una mascota.
        /// </summary>
        /// <param name="idMascota">ID de la mascota.</param>
        /// <returns>Lista de historiales clínicos.</returns>
        public List<HistorialClinico> ObtenerPorMascota(int idMascota)
        {
            return historialClinicoDAO.ObtenerPorMascota(idMascota);
        }

        /// <summary>
        /// Inserta un nuevo registro en el historial clínico.
        /// </summary>
        /// <param name="historial">Objeto historial clínico a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(HistorialClinico historial)
        {
            return historialClinicoDAO.Insertar(historial);
        }

        /// <summary>
        /// Actualiza un registro existente del historial clínico.
        /// </summary>
        /// <param name="historial">Objeto historial clínico con datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(HistorialClinico historial)
        {
            return historialClinicoDAO.Actualizar(historial);
        }
    }
}