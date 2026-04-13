using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un registro clínico asocaido a una cita registrada en el sistema.
    /// </summary>
    public class HistorialClinico
    {
        /// <summary>
        /// Identificador del registro clínico.
        /// </summary>
        public int IdHistorial { get; set; }

        /// <summary>
        /// Identificador de la mascota asociada al registro clínico.
        /// </summary>
        public int IdMascota { get; set; }

        /// <summary>
        /// Identificador de la cita asociada al registro clínico.
        /// </summary>
        public int? IdCita { get; set; }

        /// <summary>
        /// Identificador del veterinario asociado al registro clínico.
        /// </summary>
        public int IdVeterinario { get; set; }

        /// <summary>
        /// Fecha y hora a la que se ha realizado registro clínico.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Peso registrado en la consulta (si procede).
        /// </summary>
        public decimal? Peso { get; set; }

        /// <summary>
        /// Diagnóstico realizado durante la consulta.
        /// </summary>
        public string? Diagnostico { get; set; }

        /// <summary>
        /// Tratamiento indicado para la mascota.
        /// </summary>
        public string? Tratamiento { get; set; }

        /// <summary>
        /// Observaciones adicionales del veterinario.
        /// </summary>
        public string? Observaciones { get; set; }

        /// <summary>
        /// Motivo de la consulta o visita.
        /// </summary>
        public string? Motivo { get; set; }

        /// <summary>
        /// Nombre del veterinario que ha realizado el registro clínico.
        /// </summary>
        public string? NombreVeterinario { get; set; }
    }
}