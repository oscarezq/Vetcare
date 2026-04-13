using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un veterinario registrado en el sistema.
    /// </summary>
    public class Veterinario
    {
        /// <summary>
        /// Identificador del veterinario.
        /// </summary>
        public int IdVeterinario { get; set; }

        /// <summary>
        /// Identificador del usuario asignado a este veterinario.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Número de colegiado del veterinario
        /// </summary>
        public string? NumeroColegiado { get; set; }

        /// <summary>
        /// Especialidad del veterinario.
        /// </summary>
        public string? Especialidad { get; set; }

        /// <summary>
        /// Username del usuario asignado al veterinario.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Nombre del usuario asignado al veterinario.
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Apellidos del usuario asignado al veterinario.
        /// </summary>
        public string? Apellidos { get; set; }
    }
}
