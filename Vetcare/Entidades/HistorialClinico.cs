using System;

namespace Vetcare.Entidades
{
    public class HistorialClinico
    {
        public int IdHistorial { get; set; }

        public int IdMascota { get; set; }

        public int? IdCita { get; set; }

        public int IdVeterinario { get; set; }

        public DateTime FechaHora { get; set; }

        public decimal? Peso { get; set; }

        public string Diagnostico { get; set; }

        public string Tratamiento { get; set; }

        public string Observaciones { get; set; }

        public string Motivo { get; set; }

        public string NombreVeterinario { get; set; }
    }
}