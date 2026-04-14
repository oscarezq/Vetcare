using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con la entidad Veterinario.
    /// Actúa como intermediario entre la capa de presentación y la capa de acceso a datos.
    /// </summary>
    public class VeterinarioService
    {
        /// <summary>
        /// Instancia del objeto de acceso a datos para Veterinario.
        /// </summary>
        private readonly VeterinarioDAO veteDAO = new();

        /// <summary>
        /// Obtiene la lista completa de veterinarios activos.
        /// </summary>
        /// <returns>Lista de objetos Veterinario.</returns>
        public List<Veterinario> ObtenerTodos()
        {
            return veteDAO.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene la lista completa de veterinarios activos.
        /// </summary>
        /// <returns>Lista de objetos Veterinario.</returns>
        public Veterinario? ObtenerPorIdUsuario(int idVet)
        {
            return veteDAO.ObtenerPorIdUsuario(idVet);
        }

        /// <summary>
        /// Inserta un nuevo veterinario en el sistema.
        /// </summary>
        /// <param name="v">Objeto Veterinario a insertar.</param>
        /// <returns>True si la inserción fue exitosa; en caso contrario, false.</returns>
        public bool Insertar(Veterinario v)
        {
            return veteDAO.Insertar(v);
        }

        /// <summary>
        /// Actualiza la información de un veterinario existente.
        /// </summary>
        /// <param name="v">Objeto Veterinario con los datos actualizados.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        public bool Actualizar(Veterinario v)
        {
            return veteDAO.Actualizar(v);
        }

        /// <summary>
        /// Realiza un borrado lógico de un veterinario,
        /// desactivando su usuario asociado en el sistema.
        /// </summary>
        /// <param name="idVeterinario">Identificador del veterinario.</param>
        /// <returns>True si la operación fue exitosa.</returns>
        public bool BorradoLogico(int idVeterinario)
        {
            return veteDAO.BorradoLogico(idVeterinario);
        }

        public int ObtenerIdVeterinarioPorUsuario(int idUsuario)
        {
            return veteDAO.ObtenerIdVeterinarioPorUsuario(idUsuario);
        }
    }
}