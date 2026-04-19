using System;
using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los clientes.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (ClienteDAO).
    /// </summary>
    public class ClienteService
    {
        /// <summary>
        /// Instancia de acceso a datos para los clientes.
        /// </summary>
        public ClienteDAO clienteDAO = new();

        /// <summary>
        /// Obtiene todos los clientes registrados.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        public List<Cliente> ObtenerTodos()
        {
            return clienteDAO.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un cliente por su identificador.
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>El cliente encontrado o null si no existe.</returns>
        public Cliente? ObtenerPorId(int id)
        {
            return clienteDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Inserta un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="cliente">Objeto cliente a insertar.</param>
        /// <returns>True si se inserta correctamente, false en caso contrario.</returns>
        public bool Insertar(Cliente cliente)
        {
            return clienteDAO.Insertar(cliente);
        }

        /// <summary>
        /// Actualiza la información de un cliente existente.
        /// </summary>
        /// <param name="cliente">Objeto cliente con los datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Cliente cliente)
        {
            return clienteDAO.Actualizar(cliente);
        }

        /// <summary>
        /// Desactiva un cliente (baja lógica).
        /// </summary>
        /// <param name="idCliente">ID del cliente.</param>
        /// <returns>True si se desactiva correctamente, false en caso contrario.</returns>
        public bool Desactivar(int idCliente)
        {
            return clienteDAO.Desactivar(idCliente);
        }

        /// <summary>
        /// Reactiva un cliente previamente desactivado.
        /// </summary>
        /// <param name="idCliente">ID del cliente.</param>
        /// <returns>True si se reactiva correctamente, false en caso contrario.</returns>
        public bool Reactivar(int idCliente)
        {
            return clienteDAO.Reactivar(idCliente);
        }

        /// <summary>
        /// Cuenta el número total de clientes registrados.
        /// </summary>
        /// <returns>Número total de clientes.</returns>
        public int ContarClientes()
        {
            return clienteDAO.ContarClientes();
        }
    }
}