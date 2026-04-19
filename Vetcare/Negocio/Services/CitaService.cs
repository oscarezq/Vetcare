using System;
using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con las citas.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (CitaDAO).
    /// </summary>
    class CitaService
    {
        /// <summary>
        /// Instancia de acceso a datos para las citas.
        /// </summary>
        public CitaDAO citaDAO = new();

        /// <summary>
        /// Obtiene todas las citas registradas.
        /// </summary>
        /// <returns>Lista de todas las citas.</returns>
        public List<Cita> ObtenerTodas()
        {
            return citaDAO.ObtenerTodas();
        }

        /// <summary>
        /// Obtiene una cita por su identificador.
        /// </summary>
        /// <param name="id">ID de la cita.</param>
        /// <returns>La cita encontrada o null si no existe.</returns>
        public Cita? ObtenerPorId(int id)
        {
            return citaDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Obtiene las citas del día actual para un veterinario específico.
        /// </summary>
        /// <param name="id">ID del veterinario.</param>
        /// <returns>Lista de citas del día.</returns>
        public List<Cita> ObtenerCitasHoyPorVeterinario(int id)
        {
            return citaDAO.ObtenerCitasHoyPorVeterinario(id);
        }

        /// <summary>
        /// Inserta una nueva cita en la base de datos.
        /// </summary>
        /// <param name="cita">Objeto cita a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Cita cita)
        {
            return citaDAO.Insertar(cita);
        }

        /// <summary>
        /// Actualiza una cita existente.
        /// </summary>
        /// <param name="cita">Objeto cita con los datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Cita cita)
        {
            return citaDAO.Actualizar(cita);
        }

        /// <summary>
        /// Actualiza el estado de una cita.
        /// </summary>
        /// <param name="idCita">ID de la cita.</param>
        /// <param name="estado">Nuevo estado de la cita.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool ActualizarEstado(int idCita, string estado)
        {
            return citaDAO.ActualizarEstado(idCita, estado);
        }

        /// <summary>
        /// Cuenta el número total de citas del día actual.
        /// </summary>
        /// <returns>Número de citas de hoy.</returns>
        public int ContarCitasHoy()
        {
            return citaDAO.ContarCitasHoy();
        }

        /// <summary>
        /// Cuenta el número de citas del día actual para un veterinario específico.
        /// </summary>
        /// <param name="idVeterinario">ID del veterinario.</param>
        /// <returns>Número de citas de hoy para ese veterinario.</returns>
        public int ContarCitasHoyPorVeterinario(int idVeterinario)
        {
            return citaDAO.ContarCitasHoyPorVeterinario(idVeterinario);
        }

        /// <summary>
        /// Obtiene las próximas citas programadas.
        /// </summary>
        /// <returns>Lista de próximas citas.</returns>
        public List<Cita> ObtenerProximasCitas()
        {
            return citaDAO.ObtenerProximasCitas();
        }
    }
}