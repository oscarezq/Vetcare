using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class CitaService
    {
        public CitaDAO citaDAO = new();

        public List<Cita> ObtenerTodas()
        {
            return citaDAO.ObtenerTodas();
        }

        public Cita? ObtenerPorId(int id)
        {
            return citaDAO.ObtenerPorId(id);
        }

        public List<Cita> ObtenerCitasHoyPorVeterinario(int id)
        {
            return citaDAO.ObtenerCitasHoyPorVeterinario(id);
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

        public int ContarCitasHoy()
        {
            return citaDAO.ContarCitasHoy();
        }

        public int ContarCitasHoyPorVeterinario(int idVeterinario)
        {
            return citaDAO.ContarCitasHoyPorVeterinario(idVeterinario);
        }

        public List<Cita> ObtenerProximasCitas()
        {
            return citaDAO.ObtenerProximasCitas();
        }
    }
}