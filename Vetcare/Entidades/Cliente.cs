using System;

namespace Vetcare.Entidades
{
    /// <summary>
    /// Clase que representa un cliente registrado en el sistema.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador del cliente.
        /// </summary>
        public int IdCliente { get; set; }

        public string? TipoDocumento { get; set; }

        /// <summary>
        /// Número de identificación del cliente (DNI, NIE, NIF...).
        /// </summary>
        public string? NumDocumento { get; set; }

        /// <summary>
        /// Nombre del cliente.
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Apellidos del cliente.
        /// </summary>
        public string? Apellidos { get; set; }

        /// <summary>
        /// Teléfono del cliente.
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Email del cliente.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Dirección del cliente.
        /// </summary>
        public string? CalleDireccion { get; set; }

        public string? NumeroDireccion { get; set; }

        public string? PisoPuertaDireccion { get; set; }

        public string? CodigoPostalDireccion { get; set; }

        public string? LocalidadDireccion { get; set; }

        public string? ProvinciaDireccion { get; set; }

        /// <summary>
        /// Fecha de alta del cliente.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Nombre + Apellidos del cliente.
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        /// <summary>
        /// Nombre + Apellidos + Número de identificación del cliente.
        /// </summary>
        public string ClienteCompleto => $"{Nombre} {Apellidos} ({NumDocumento})";
    }

}
