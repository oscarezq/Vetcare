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
        /// Método para insertar varios clientes
        /// </summary>
        /// <param name="clientes">Lista de clientes que se van a insertar</param>
        /// <returns>Booleano que indica si se han insertado correctamente</returns>
        public bool InsertarVarios(List<Cliente> clientes)
        {
            return clienteDAO.InsertarVarios(clientes);
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
        /// Método para actualizar varios clientes.
        /// </summary>
        /// <param name="clientes">Lista de clientes con los datos actualizados.</param>
        /// <returns>Booleano que indica si se han actualizado correctamente.</returns>
        public bool ActualizarVarios(List<Cliente> clientes)
        {
            return clienteDAO.ActualizarVarios(clientes);
        }

        /// <summary>
        /// Método para eliminar un cliente
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Booleano que indica si se ha eliminado correctamente</returns>
        public bool Eliminar(int idCliente)
        {
            return clienteDAO.Eliminar(idCliente);
        }

        /// <summary>
        /// Método para eliminar varios clientes
        /// </summary>
        /// <param name="idsClientes">Lista de identificadores de los clientes a eliminar</param>
        /// <returns>Booleano que indica si se han eliminado correctamente</returns>
        public bool EliminarVarias(List<int> idsClientes)
        {
            return clienteDAO.EliminarVarios(idsClientes);
        }
    }
}

