using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class CitaService
    {
        public CitaDAO citaDAO = new CitaDAO();

        public List<Cita> ObtenerTodas()
        {
            return citaDAO.ObtenerTodas();
        }

        public Cita ObtenerPorId(int id)
        {
            return citaDAO.ObtenerPorId(id);
        }

        public bool Insertar(Cita cita)
        {
            return citaDAO.Insertar(cita);
        }

        public bool InsertarVarios(List<Cita> citas)
        {
            return citaDAO.InsertarVarias(citas);
        }

        public bool Actualizar(Cita cita)
        {
            return citaDAO.Actualizar(cita);
        }

        public bool ActualizarVarios(List<Cita> citas)
        {
            return citaDAO.ActualizarVarias(citas);
        }

        public bool Eliminar(int idCita)
        {
            return citaDAO.Eliminar(idCita);
        }

        public bool EliminarVarias(List<int> ids)
        {
            return citaDAO.EliminarVarias(ids);
        }
    }
}