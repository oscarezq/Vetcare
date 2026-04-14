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

        public List<Raza> ObtenerPorEspecie(int idEspecie)
        {
            return razaDAO.ObtenerPorEspecie(idEspecie);
        }

        public bool Insertar(Raza raza)
        {
            return razaDAO.Insertar(raza);
        }
    }
}