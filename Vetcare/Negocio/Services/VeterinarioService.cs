using System;
using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los veterinarios.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (VeterinarioDAO).
    /// </summary>
    public class VeterinarioService
    {
        /// <summary>
        /// Instancia de acceso a datos para los veterinarios.
        /// </summary>
        private readonly VeterinarioDAO veteDAO = new();

        /// <summary>
        /// Obtiene todos los veterinarios registrados.
        /// </summary>
        /// <returns>Lista de veterinarios.</returns>
        public List<Veterinario> ObtenerTodos()
        {
            return veteDAO.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un veterinario asociado a un usuario específico.
        /// </summary>
        /// <param name="idVet">ID del usuario o veterinario asociado.</param>
        /// <returns>Veterinario encontrado o null si no existe.</returns>
        public Veterinario? ObtenerPorIdUsuario(int idVet)
        {
            return veteDAO.ObtenerPorIdUsuario(idVet);
        }

        /// <summary>
        /// Inserta un nuevo veterinario en la base de datos.
        /// </summary>
        /// <param name="v">Objeto veterinario a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Veterinario v)
        {
            return veteDAO.Insertar(v);
        }

        /// <summary>
        /// Actualiza los datos de un veterinario existente.
        /// </summary>
        /// <param name="v">Objeto veterinario con datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Veterinario v)
        {
            return veteDAO.Actualizar(v);
        }

        /// <summary>
        /// Realiza el borrado lógico de un veterinario.
        /// </summary>
        /// <param name="idVeterinario">ID del veterinario.</param>
        /// <returns>True si se elimina correctamente, false en caso contrario.</returns>
        public bool BorradoLogico(int idVeterinario)
        {
            return veteDAO.BorradoLogico(idVeterinario);
        }

        /// <summary>
        /// Obtiene el ID del veterinario asociado a un usuario.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>ID del veterinario asociado.</returns>
        public int ObtenerIdVeterinarioPorUsuario(int idUsuario)
        {
            return veteDAO.ObtenerIdVeterinarioPorUsuario(idUsuario);
        }
    }
}