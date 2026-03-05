using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class EspecieService
    {
        public EspecieDAO especieDAO = new EspecieDAO();

        public List<Especie> ObtenerTodas()
        {
            return especieDAO.ObtenerTodas();
        }

        public Especie ObtenerPorId(int id)
        {
            return especieDAO.ObtenerPorId(id);
        }

        public bool Insertar(Especie especie)
        {
            return especieDAO.Insertar(especie);
        }

        public bool Actualizar(Especie especie)
        {
            return especieDAO.Actualizar(especie);
        }

        public bool Eliminar(int idEspecie)
        {
            return especieDAO.Eliminar(idEspecie);
        }
    }
}