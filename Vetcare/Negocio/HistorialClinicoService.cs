using System.Collections.Generic;
using Vetcare.DAO;
using Vetcare.Entidades;

namespace Vetcare.Service
{
    public class HistorialClinicoService
    {
        private HistorialClinicoDAO dao = new HistorialClinicoDAO();

        public void Crear(HistorialClinico historial)
        {
            dao.Insertar(historial);
        }

        public HistorialClinico ObtenerPorId(int id)
        {
            return dao.ObtenerPorId(id);
        }

        public HistorialClinico ObtenerPorIdCita(int id)
        {
            return dao.ObtenerPorIdCita(id);
        }

        public List<HistorialClinico> ObtenerTodos()
        {
            return dao.ListarTodos();
        }

        public List<HistorialClinico> ObtenerPorMascota(int idMascota)
        {
            return dao.ListarPorMascota(idMascota);
        }

        public bool Insertar(HistorialClinico historial)
        {
            return dao.Insertar(historial);
        }

        public bool Actualizar(HistorialClinico historial)
        {
            return dao.Actualizar(historial);
        }

        public void Eliminar(int id)
        {
            dao.Eliminar(id);
        }
    }
}