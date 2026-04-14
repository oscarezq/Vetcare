using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con las mascotas.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (MascotaDAO).
    /// </summary>
    class MascotaService
    {
        /// <summary>
        /// Instancia de acceso a datos para las mascotas.
        /// </summary>
        public MascotaDAO mascotaDAO = new();

        /// <summary>
        /// Obtiene todas las mascotas registradas.
        /// </summary>
        /// <returns>Lista de mascotas.</returns>
        public List<Mascota> ObtenerTodas()
        {
            return mascotaDAO.ObtenerTodas();
        }

        /// <summary>
        /// Obtiene una mascota por su identificador.
        /// </summary>
        /// <param name="id">ID de la mascota.</param>
        /// <returns>La mascota encontrada o null si no existe.</returns>
        public Mascota? ObtenerPorId(int id)
        {
            return mascotaDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Obtiene todas las mascotas asociadas a un cliente.
        /// </summary>
        /// <param name="idCliente">ID del cliente.</param>
        /// <returns>Lista de mascotas del cliente.</returns>
        public List<Mascota> ObtenerPorCliente(int idCliente)
        {
            return mascotaDAO.ObtenerPorCliente(idCliente);
        }

        /// <summary>
        /// Inserta una nueva mascota en la base de datos.
        /// </summary>
        /// <param name="mascota">Objeto mascota a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Mascota mascota)
        {
            return mascotaDAO.Insertar(mascota);
        }

        /// <summary>
        /// Actualiza los datos de una mascota existente.
        /// </summary>
        /// <param name="mascota">Objeto mascota con datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Mascota mascota)
        {
            return mascotaDAO.Actualizar(mascota);
        }

        /// <summary>
        /// Desactiva una mascota (baja lógica).
        /// </summary>
        /// <param name="idMascota">ID de la mascota.</param>
        /// <returns>True si se desactiva correctamente, false en caso contrario.</returns>
        public bool Desactivar(int idMascota)
        {
            return mascotaDAO.Desactivar(idMascota);
        }

        /// <summary>
        /// Desactiva varias mascotas a la vez.
        /// </summary>
        /// <param name="idsMascotas">Lista de IDs de mascotas a desactivar.</param>
        /// <returns>True si la operación se realiza correctamente.</returns>
        public bool DesactivarVarios(List<int> idsMascotas)
        {
            return mascotaDAO.DesactivarVarios(idsMascotas);
        }

        /// <summary>
        /// Reactiva una mascota previamente desactivada.
        /// </summary>
        /// <param name="idMascota">ID de la mascota.</param>
        /// <returns>True si se reactiva correctamente, false en caso contrario.</returns>
        public bool Reactivar(int idMascota)
        {
            return mascotaDAO.Reactivar(idMascota);
        }

        /// <summary>
        /// Cuenta el número total de mascotas registradas.
        /// </summary>
        /// <returns>Número total de mascotas.</returns>
        public int ContarMascotas()
        {
            return mascotaDAO.ContarMascotas();
        }
    }
}