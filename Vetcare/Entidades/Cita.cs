using System;

namespace TFG.CapaEntidad
{
    /// <summary>
    /// Clase que representa una cita veterinaria.
    /// </summary>
    public class Cita
    {
        /// <summary>
        /// Identificador de la cita.
        /// </summary>
        public int IdCita { get; set; }

        /// <summary>
        /// Identificador de la mascota que va a ser atendida.
        /// </summary>
        public int IdMascota { get; set; }
        
        /// <summary>
        /// Identificador del veterinario que va a atender a la mascota.
        /// </summary>
        public int IdVeterinario { get; set; }

        /// <summary>
        /// Fecha y hora de la cita.
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Motivo de la cita.
        /// </summary>
        public string Motivo { get; set; }

        /// <summary>
        /// Estado de la cita ('pendiente', 'cancelada', 'completada').
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Observaciones asociadas a la cita.
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Nombre de la mascota.
        /// </summary>
        public string NombreMascota { get; set; }

        /// <summary>
        /// Nombre del dueño de la mascota.
        /// </summary>
        public string NombreDueno { get; set; }

        /// <summary>
        /// Nombre del veterinario.
        /// </summary>
        public string NombreVeterinario { get; set; }
    }
}