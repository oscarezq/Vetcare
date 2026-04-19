using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con las especies.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (EspecieDAO).
    /// </summary>
    class EspecieService
    {
        /// <summary>
        /// Instancia de acceso a datos para las especies.
        /// </summary>
        public EspecieDAO especieDAO = new();

        /// <summary>
        /// Obtiene todas las especies registradas.
        /// </summary>
        /// <returns>Lista de especies.</returns>
        public List<Especie> ObtenerTodas()
        {
            return especieDAO.ObtenerTodas();
        }

        /// <summary>
        /// Inserta una nueva especie en la base de datos.
        /// </summary>
        /// <param name="especie">Objeto especie a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Especie especie)
        {
            return especieDAO.Insertar(especie);
        }
    }
}