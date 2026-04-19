using System;
using System.Collections.Generic;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Services
{
    /// <summary>
    /// Servicio encargado de gestionar la lógica de negocio relacionada con los usuarios.
    /// Incluye autenticación, gestión de usuarios y validaciones básicas.
    /// Actúa como intermediario entre la capa de presentación y la capa de datos (UsuarioDAO).
    /// </summary>
    public class UsuarioService
    {
        /// <summary>
        /// Instancia de acceso a datos para los usuarios.
        /// </summary>
        private readonly UsuarioDAO usuarioDAO = new();

        /// <summary>
        /// Valida el login de un usuario comprobando credenciales y devolviendo el usuario autenticado.
        /// </summary>
        /// <param name="user">Nombre de usuario.</param>
        /// <param name="pass">Contraseña del usuario.</param>
        /// <param name="usuarioLogueado">Usuario autenticado si las credenciales son correctas.</param>
        /// <returns>Mensaje de error o cadena vacía si el login es correcto.</returns>
        public string ValidarLogin(string user, string pass, out Usuario? usuarioLogueado)
        {
            usuarioLogueado = null;
            string mensaje = string.Empty;

            try
            {
                usuarioLogueado = usuarioDAO.Login(user, pass);

                if (usuarioLogueado == null)
                {
                    mensaje = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error de conexión: " + ex.Message;
            }

            return mensaje;
        }

        /// <summary>
        /// Obtiene todos los usuarios registrados.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        public List<Usuario> ObtenerTodos()
        {
            return usuarioDAO.ObtenerTodos();
        }

        /// <summary>
        /// Obtiene un usuario por su identificador.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Usuario encontrado o null si no existe.</returns>
        public Usuario? ObtenerPorId(int id)
        {
            return usuarioDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Comprueba si ya existe algún usuario en la base de datos.
        /// </summary>
        /// <returns>True si hay usuarios, false si no hay usuarios</returns>
        public bool ComprobarHayUsuarios()
        {
            return usuarioDAO.ComprobarHayUsuarios();
        }

        /// <summary>
        /// Inserta en la base de datos el usuario admin.
        /// </summary>
        /// <returns>True si se ha insertado correctamente, false si no se ha insertado</returns>
        public bool InsertarUsuarioAdmin(string hash, string salt)
        {
            return usuarioDAO.InsertarUsuarioAdmin(hash, salt);
        }

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="u">Objeto usuario a insertar.</param>
        /// <returns>ID del usuario insertado o código de resultado.</returns>
        public int Insertar(Usuario u)
        {
            return usuarioDAO.Insertar(u);
        }

        /// <summary>
        /// Actualiza la información de un usuario existente.
        /// </summary>
        /// <param name="u">Objeto usuario con datos actualizados.</param>
        /// <returns>True si se actualiza correctamente, false en caso contrario.</returns>
        public bool Actualizar(Usuario u)
        {
            return usuarioDAO.Actualizar(u);
        }

        /// <summary>
        /// Realiza el borrado lógico de un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <returns>True si se elimina correctamente, false en caso contrario.</returns>
        public bool Eliminar(int id)
        {
            return usuarioDAO.BorradoLogico(id);
        }

        /// <summary>
        /// Comprueba si ya existe un nombre de usuario en la base de datos.
        /// </summary>
        /// <param name="username">Nombre de usuario a verificar.</param>
        /// <returns>True si existe, false si no existe.</returns>
        public bool ExisteUsername(string username)
        {
            return usuarioDAO.ExisteUsername(username);
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>True si se reactiva correctamente, false en caso contrario.</returns>
        public bool Reactivar(int idUsuario)
        {
            return usuarioDAO.Reactivar(idUsuario);
        }
    }
}