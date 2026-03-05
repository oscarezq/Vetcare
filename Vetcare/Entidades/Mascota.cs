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
        public string NumeroChip { get; set; }

        /// <summary>
        /// Nombre de la mascota.
        /// </summary>
        public string Nombre { get; set; }

        public int IdEspecie { get; set; }
        public int IdRaza { get; set; }

        public string NombreEspecie { get; set; }
        public string NombreRaza { get; set; }

        /// <summary>
        /// Sexo de la mascota (macho, hembra).
        /// </summary>
        public string Sexo { get; set; }

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
        public string NombreDueno { get; set; }

        /// <summary>
        /// Apellidos del dueño de la mascota.
        /// </summary>
        public string ApellidosDueno { get; set; }

        /// <summary>
        /// Número de identificación del dueño de la mascota.
        /// </summary>
        public string NumeroIdentificacionDueno { get; set; }

        /// <summary>
        /// Nombre + Apellidos + (Identificador) del dueño.
        /// </summary>
        public string Dueno => $"{this.NombreDueno} {this.ApellidosDueno} ({this.NumeroIdentificacionDueno})";
    }
}