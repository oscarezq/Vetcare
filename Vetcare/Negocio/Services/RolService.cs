using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los roles.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (RolDAO).
    /// </summary>
    public class RolService
    {
        /// <summary>
        /// Instancia de acceso a datos para los roles.
        /// </summary>
        private readonly RolDAO rolDAO = new();

        /// <summary>
        /// Obtiene la lista de todos los roles disponibles.
        /// </summary>
        /// <returns>Lista de roles.</returns>
        public List<Rol> ListarRoles()
        {
            return rolDAO.Listar();
        }
    }
}