using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Clase encargada de gestionar la lógica de negocio relacionada con la entidad Mascota.
    /// </summary>
    class MascotaService
    {
        // Objeto DAO para acceder a los datos
        public MascotaDAO mascotaDAO = new MascotaDAO();

        /// <summary>
        /// Método para obtener todas las mascotas
        /// </summary>
        /// <returns>Lista con todas las mascotas</returns>
        public List<Mascota> ObtenerTodas()
        {
            return mascotaDAO.ObtenerTodas();
        }

        /// <summary>
        /// Método para obtener una mascota por su identificador
        /// </summary>
        /// <returns>Mascota con el identificador correspondiente</returns>
        public Mascota ObtenerPorId(int id)
        {
            return mascotaDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Método para obtener las mascotas de un cliente concreto
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Lista de mascotas pertenecientes al cliente</returns>
        public List<Mascota> ObtenerPorCliente(int idCliente)
        {
            return mascotaDAO.ObtenerPorCliente(idCliente);
        }

        /// <summary>
        /// Método para insertar una mascota
        /// </summary>
        /// <param name="mascota">Mascota que se va a insertar</param>
        /// <returns>Booleano que indica si se ha insertado correctamente</returns>
        public bool Insertar(Mascota mascota)
        {
            return mascotaDAO.Insertar(mascota);
        }

        /// <summary>
        /// Método para insertar varias mascotas
        /// </summary>
        /// <param name="mascotas">Lista de mascotas que se van a insertar</param>
        /// <returns>Booleano que indica si se han insertado correctamente</returns>
        public bool InsertarVarios(List<Mascota> mascotas)
        {
            return mascotaDAO.InsertarVarios(mascotas);
        }

        /// <summary>
        /// Método para actualizar una mascota
        /// </summary>
        /// <param name="mascota">Mascota con los datos actualizados</param>
        /// <returns>Booleano que indica si se ha actualizado correctamente</returns>
        public bool Actualizar(Mascota mascota)
        {
            return mascotaDAO.Actualizar(mascota);
        }

        /// <summary>
        /// Método para actualizar varias mascotas
        /// </summary>
        /// <param name="mascotas">Lista de mascotas con los datos actualizados</param>
        /// <returns>Booleano que indica si se han actualizado correctamente</returns>
        public bool ActualizarVarios(List<Mascota> mascotas)
        {
            return mascotaDAO.ActualizarVarios(mascotas);
        }

        /// <summary>
        /// Método para eliminar una mascota
        /// </summary>
        /// <param name="idMascota">Identificador de la mascota</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente</returns>
        public bool Eliminar(int idMascota)
        {
            return mascotaDAO.Eliminar(idMascota);
        }

        /// <summary>
        /// Método para eliminar varias mascotas
        /// </summary>
        /// <param name="idsMascotas">Lista de identificadores de las mascotas a eliminar</param>
        /// <returns>Booleano que indica si se han eliminado correctamente</returns>
        public bool EliminarVarios(List<int> idsMascotas)
        {
            return mascotaDAO.EliminarVarios(idsMascotas);
        }
    }
}