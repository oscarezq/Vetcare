using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    public class ConceptoService
    {
        private ConceptoDAO servicioDAO = new ConceptoDAO();

        public List<Concepto> ObtenerTodos()
        {
            return servicioDAO.ObtenerTodos();
        }

        public List<Concepto> ObtenerServicios()
        {
            return servicioDAO.ObtenerServicios();
        }

        public List<Concepto> ObtenerProductos()
        {
            return servicioDAO.ObtenerProductos();
        }

        public bool Insertar(Concepto servicio)
        {
            return servicioDAO.Insertar(servicio);
        }

        public bool Actualizar(Concepto servicio)
        {
            return servicioDAO.Actualizar(servicio);
        }

    }
}
