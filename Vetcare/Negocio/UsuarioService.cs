using System;
using System.Collections.Generic;
using Vetcare.Datos;
using Vetcare.Entidades;

namespace Vetcare.Negocio
{
    /// <summary>
    /// Lógica de negocio para la gestión de usuarios.
    /// </summary>
    public class UsuarioService
    {
        // Instanciamos el DAO para comunicarnos con la base de datos
        private UsuarioDAO _usuarioDAO = new UsuarioDAO();

        /// <summary>
        /// Realiza las validaciones de negocio para el inicio de sesión.
        /// </summary>
        /// <param name="user">Username introducido.</param>
        /// <param name="pass">Password introducida.</param>
        /// <param name="usuarioLogueado">Parámetro de salida que contendrá el objeto Usuario si el login es correcto.</param>
        /// <returns>Un mensaje de error detallado o una cadena vacía si el login es exitoso.</returns>
        public string ValidarLogin(string user, string pass, out Usuario usuarioLogueado)
        {
            usuarioLogueado = null;
            string mensaje = string.Empty;

            // Validaciones previas
            if (string.IsNullOrWhiteSpace(user))
                return "El nombre de usuario es obligatorio.";

            if (string.IsNullOrWhiteSpace(pass))
                return "La contraseña es obligatoria.";

            try
            {
                // Llamada al DAO para verificar credenciales
                usuarioLogueado = _usuarioDAO.Login(user, pass);

                // Si el DAO devuelve null, es que el usuario no existe, la clave está mal o está inactivo
                if (usuarioLogueado == null)
                {
                    mensaje = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                // Capturamos errores de conexión o SQL
                mensaje = "Error de conexión: " + ex.Message;
            }

            return mensaje;
        }

        /// <summary>
        /// Obtiene la lista de todos los usuarios activos en el sistema.
        /// </summary>
        /// <returns>Lista de objetos Usuario.</returns>
        public List<Usuario> ObtenerTodos()
        {
            try
            {
                return _usuarioDAO.ObtenerTodos();
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear el error
                throw new Exception("Error en capa de negocio al listar usuarios: " + ex.Message);
            }
        }

        /// <summary>
        /// Busca un usuario por su identificador único.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Objeto Usuario o null si no se encuentra.</returns>
        public Usuario ObtenerPorId(int id)
        {
            if (id <= 0) return null;
            return _usuarioDAO.ObtenerPorId(id);
        }

        /// <summary>
        /// Inserta un nuevo usuario realizando validaciones previas.
        /// </summary>
        /// <param name="u">Objeto Usuario a insertar.</param>
        /// <returns>Indice del último usuario que se ha insertado.</returns>
        public int Insertar(Usuario u)
        {
            // Regla de negocio: El username es obligatorio
            if (string.IsNullOrWhiteSpace(u.Username))
                throw new Exception("El nombre de usuario es obligatorio.");

            // Regla de negocio: El email debe tener un formato mínimo (opcional)
            if (!string.IsNullOrEmpty(u.Email) && !u.Email.Contains("@"))
                throw new Exception("El formato del correo electrónico no es válido.");

            return _usuarioDAO.Insertar(u);
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario existente.
        /// </summary>
        /// <param name="u">Objeto Usuario.</param>
        /// <returns>True si se actualizó la contraseña con éxito.</returns>
        public bool Actualizar(Usuario u)
        {
            return _usuarioDAO.Actualizar(u);
        }

        /// <summary>
        /// Realiza el borrado lógico de un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a desactivar.</param>
        /// <returns>True si se desactivó correctamente.</returns>
        public bool Eliminar(int id)
        {
            if (id <= 0) return false;
            return _usuarioDAO.BorradoLogico(id);
        }

        public bool ExisteUsername(string username)
        {
            return _usuarioDAO.ExisteUsername(username);
        }

        public bool Reactivar(int idUsuario)
        {
            return _usuarioDAO.Reactivar(idUsuario);
        }
    }
}
