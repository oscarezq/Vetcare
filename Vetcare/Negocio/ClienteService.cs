using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Clase encargada de gestionar la lógica de negocio relacionada con la entidad Cliente.
    /// </summary>
    public class ClienteService
    {
        // Objeto DAO para acceder a los datos
        public ClienteDAO clienteDAO = new ClienteDAO();

        /// <summary>
        /// Método para obtener todos los clientes
        /// </summary>
        /// <returns>Lista con todos los clientes</returns>
        public List<Cliente> ObtenerTodos()
        {
            return clienteDAO.ObtenerTodos();
        }

        /// <summary>
        ///Método para obtener un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente a obtener.</param>
        /// <returns>Cliente si existe; de lo contrario, null.</returns>
        public Cliente ObtenerPorId(int id)
        {
            return clienteDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Método para insertar un cliente
        /// </summary>
        /// <param name="cliente">Cliente que se va a insertar</param>
        /// <returns>Booleano que indica si se ha insertado correctamente</returns>
        public bool Insertar(Cliente cliente)
        {
            return clienteDAO.Insertar(cliente);
        }

        /// <summary>
        /// Método para actualizar un cliente existente.
        /// </summary>
        /// <param name="cliente">Cliente con los datos actualizados.</param>
        /// <returns>Booleano que indica si se ha actualizado correctamente.</returns>
        public bool Actualizar(Cliente cliente)
        {
            return clienteDAO.Actualizar(cliente);
        }

        /// <summary>
        /// Método para eliminar un cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente</returns>
        public bool Desactivar(int idCliente)
        {
            return clienteDAO.Desactivar(idCliente);
        }

        /// <summary>
        /// Método para eliminar un cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente</returns>
        public bool Reactivar(int idCliente)
        {
            return clienteDAO.Reactivar(idCliente);
        }

        public int ContarClientes()
        {
            return clienteDAO.ContarClientes();
        }
    }
}

