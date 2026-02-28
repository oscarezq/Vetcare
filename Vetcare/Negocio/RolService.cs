using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    public class RolService
    {
        private RolDAO _rolDAO = new RolDAO();

        public List<Rol> ListarRoles()
        {
            return _rolDAO.Listar();
        }
    }
}