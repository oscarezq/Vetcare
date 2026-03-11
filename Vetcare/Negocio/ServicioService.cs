using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    public class ServicioService
    {
        private ServicioDAO servicioDAO = new ServicioDAO();

        public List<Servicio> ObtenerTodos()
        {
            return servicioDAO.ObtenerTodos();
        }

        public bool Insertar(Servicio servicio)
        {
            return servicioDAO.Insertar(servicio);
        }

        public bool Actualizar(Servicio servicio)
        {
            return servicioDAO.Actualizar(servicio);
        }
    }
}
