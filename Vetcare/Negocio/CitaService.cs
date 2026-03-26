using System;
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

        public bool Actualizar(Cita cita)
        {
            return citaDAO.Actualizar(cita);
        }

        public bool ActualizarEstado(int idCita, string estado)
        {
            return citaDAO.ActualizarEstado(idCita, estado);
        }

        public bool Eliminar(int idCita)
        {
            return citaDAO.Eliminar(idCita);
        }

        public bool EliminarVarias(List<int> ids)
        {
            return citaDAO.EliminarVarias(ids);
        }

        public int ContarCitasHoy()
        {
            return citaDAO.ContarCitasHoy();
        }

        public List<Cita> ObtenerProximasCitas()
        {
            return citaDAO.ObtenerProximasCitas();
        }
    }
}