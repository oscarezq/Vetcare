using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    public class RolService
    {
        private readonly RolDAO _rolDAO = new();

        public List<Rol> ListarRoles()
        {
            return _rolDAO.Listar();
        }
    }
}