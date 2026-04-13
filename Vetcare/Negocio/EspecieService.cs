using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    class EspecieService
    {
        public EspecieDAO especieDAO = new();

        public List<Especie> ObtenerTodas()
        {
            return especieDAO.ObtenerTodas();
        }

        public bool Insertar(Especie especie)
        {
            return especieDAO.Insertar(especie);
        }
    }
}