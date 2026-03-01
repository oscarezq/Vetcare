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
        private VeterinarioDAO veteDAO = new VeterinarioDAO();

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
        public Veterinario ObtenerPorId(int idVet)
        {
            return veteDAO.ObtenerPorId(idVet);
        }

        /// <summary>
        /// Obtiene la lista completa de veterinarios activos.
        /// </summary>
        /// <returns>Lista de objetos Veterinario.</returns>
        public Veterinario ObtenerPorIdUsuario(int idVet)
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
        /// Inserta múltiples veterinarios en el sistema.
        /// </summary>
        /// <param name="lista">Lista de veterinarios a insertar.</param>
        /// <returns>True si todas las inserciones fueron exitosas.</returns>
        public bool InsertarVarios(List<Veterinario> lista)
        {
            return veteDAO.InsertarVarios(lista);
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
        /// Actualiza múltiples veterinarios en el sistema.
        /// </summary>
        /// <param name="lista">Lista de veterinarios a actualizar.</param>
        /// <returns>True si todas las actualizaciones fueron exitosas.</returns>
        public bool ActualizarVarios(List<Veterinario> lista)
        {
            return veteDAO.ActualizarVarios(lista);
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

        /// <summary>
        /// Realiza el borrado lógico de varios veterinarios
        /// dentro de una misma operación transaccional.
        /// </summary>
        /// <param name="ids">Lista de identificadores de veterinarios.</param>
        /// <returns>True si todos fueron desactivados correctamente.</returns>
        public bool BorradoLogicoVarios(List<int> ids)
        {
            return veteDAO.BorradoLogicoVarios(ids);
        }
    }
}