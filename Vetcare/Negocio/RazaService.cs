using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con las razas.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (RazaDAO).
    /// </summary>
    class RazaService
    {
        /// <summary>
        /// Instancia de acceso a datos para las razas.
        /// </summary>
        public RazaDAO razaDAO = new();

        /// <summary>
        /// Obtiene todas las razas registradas.
        /// </summary>
        /// <returns>Lista de razas.</returns>
        public List<Raza> ObtenerTodas()
        {
            return razaDAO.ObtenerTodas();
        }

        /// <summary>
        /// Obtiene todas las razas pertenecientes a una especie concreta.
        /// </summary>
        /// <param name="idEspecie">ID de la especie.</param>
        /// <returns>Lista de razas filtradas por especie.</returns>
        public List<Raza> ObtenerPorEspecie(int idEspecie)
        {
            return razaDAO.ObtenerPorEspecie(idEspecie);
        }

        /// <summary>
        /// Inserta una nueva raza en la base de datos.
        /// </summary>
        /// <param name="raza">Objeto raza a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Raza raza)
        {
            return razaDAO.Insertar(raza);
        }
    }
}