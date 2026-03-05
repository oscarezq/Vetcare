using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class RazaService
    {
        public RazaDAO razaDAO = new RazaDAO();

        public List<Raza> ObtenerTodas()
        {
            return razaDAO.ObtenerTodas();
        }

        public Raza ObtenerPorId(int id)
        {
            return razaDAO.ObtenerPorId(id);
        }

        public bool Insertar(Raza raza)
        {
            return razaDAO.Insertar(raza);
        }

        public bool Actualizar(Raza raza)
        {
            return razaDAO.Actualizar(raza);
        }

        public bool Eliminar(int idRaza)
        {
            return razaDAO.Eliminar(idRaza);
        }
    }
}