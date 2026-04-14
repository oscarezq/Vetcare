using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Service
{
    public class HistorialClinicoService
    {
        private readonly HistorialClinicoDAO dao = new();

        public HistorialClinico? ObtenerPorIdCita(int id)
        {
            return dao.ObtenerPorIdCita(id);
        }

        public List<HistorialClinico> ObtenerPorMascota(int idMascota)
        {
            return dao.ObtenerPorMascota(idMascota);
        }

        public bool Insertar(HistorialClinico historial)
        {
            return dao.Insertar(historial);
        }

        public bool Actualizar(HistorialClinico historial)
        {
            return dao.Actualizar(historial);
        }
    }
}