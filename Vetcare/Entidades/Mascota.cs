using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa una mascota registrada en el sistema.
    /// </summary>
    public class Mascota
    {
        /// <summary>
        /// Identificador de la mascota.
        /// </summary>
        public int IdMascota { get; set; }

        /// <summary>
        /// Identificador del cliente (dueño de la mascota).
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Número de chip de la mascota.
        /// </summary>
        public string? NumeroChip { get; set; }

        /// <summary>
        /// Nombre de la mascota.
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Identificador de la especie a la que pertenece la mascota.
        /// </summary>
        public int IdEspecie { get; set; }

        /// <summary>
        /// Nombre de la especie a la que pertenece la mascota.
        /// </summary>
        public string? NombreEspecie { get; set; }

        /// <summary>
        /// Identificador de la raza la que pertenece la mascota.
        /// </summary>
        public int IdRaza { get; set; }

        /// <summary>
        /// Nombre de la raza a la que pertenece la mascota.
        /// </summary>
        public string? NombreRaza { get; set; }

        /// <summary>
        /// Sexo de la mascota ('Macho', 'Hembra').
        /// </summary>
        public string? Sexo { get; set; }

        /// <summary>
        /// Peso de la mascota en kilogramos.
        /// </summary>
        public decimal Peso { get; set; }

        /// <summary>
        /// Fecha de nacimiento de la mascota.
        /// </summary>
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Nombre del dueño de la mascota.
        /// </summary>
        public string? NombreDueno { get; set; }

        /// <summary>
        /// Apellidos del dueño de la mascota.
        /// </summary>
        public string? ApellidosDueno { get; set; }

        /// <summary>
        /// Número de identificación del dueño de la mascota.
        /// </summary>
        public string? NumeroIdentificacionDueno { get; set; }

        /// <summary>
        /// Nombre + Apellidos + Número de identificación del dueño.
        /// </summary>
        public string? Dueno => $"{this.NombreDueno} {this.ApellidosDueno} ({this.NumeroIdentificacionDueno})";

        /// <summary>
        /// Indica si la mascota está activa en el sistema.
        /// </summary>
        public bool Activo { get; set; }
    }
}