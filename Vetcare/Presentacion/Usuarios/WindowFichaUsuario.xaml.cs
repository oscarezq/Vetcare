using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Usuarios
{
    /// <summary>
    /// Ventana que muestra la ficha detallada de un usuario
    /// Incluye datos personales, acceso y datos profesionales (si aplica)
    /// </summary>
    public partial class WindowFichaUsuario : Window
    {
        // Servicio para gestionar usuarios
        private readonly UsuarioService _usuarioService = new();

        // Servicio para gestionar veterinarios
        private readonly VeterinarioService _veteService = new();

        // Usuario que se está mostrando actualmente en la ficha
        private Usuario? _usuarioActual;

        /// <summary>
        /// Constructor que recibe el ID del usuario a mostrar
        /// </summary>
        public WindowFichaUsuario(int idUsuario)
        {
            InitializeComponent();

            // Cargamos los datos del usuario
            CargarDatos(idUsuario);
        }

        /// <summary>
        /// Carga los datos del usuario y los muestra en la interfaz
        /// </summary>
        private void CargarDatos(int idUsuario)
        {
            // Obtenemos el usuario desde la base de datos
            _usuarioActual = _usuarioService.ObtenerPorId(idUsuario);

            // Si no existe, mostramos error y cerramos ventana
            if (_usuarioActual == null)
            {
                MessageBox.Show("Usuario no encontrado.");
                this.Close();
                return;
            }

            // Asignamos el usuario al DataContext para el binding en XAML
            this.DataContext = _usuarioActual;

            // --- LÓGICA DE RESTRICCIÓN ---
            // Ocultamos botón de editar si:
            // - El usuario está inactivo
            // - El usuario logueado es veterinario (rol 2)
            if (!_usuarioActual.Activo || (Sesion.UsuarioActual != null && Sesion.UsuarioActual.IdRol == 2))
            {
                btnEditarUsuario.Visibility = Visibility.Collapsed;
            }
            else
            {
                // En caso contrario, mostramos el botón
                btnEditarUsuario.Visibility = Visibility.Visible;
            }

            // --- INFORMACIÓN PROFESIONAL ---
            // Solo mostramos esta sección si el usuario es veterinario
            if (_usuarioActual.NombreRol == "Veterinario")
            {
                // Obtenemos los datos del veterinario asociados al usuario
                var vet = _veteService.ObtenerPorIdUsuario(_usuarioActual.IdUsuario);

                // Si existe información profesional
                if (vet != null)
                {
                    // Mostramos la sección
                    seccionProfesional.Visibility = Visibility.Visible;

                    // Asignamos los valores a la UI
                    txtNumeroColegiado.Text = vet.NumeroColegiado;
                    txtEspecialidad.Text = vet.Especialidad;
                }
            }
            else
            {
                seccionProfesional.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Evento que se ejecuta al pulsar el botón de editar usuario
        /// </summary>
        private void BtnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioActual != null)
            {
                // Abrimos la ventana de edición pasando el usuario actual
                WindowUsuario winEdit = new(_usuarioActual)
                {
                    Owner = Window.GetWindow(this)
                };

                // Si el usuario guarda cambios
                if (winEdit.ShowDialog() == true)
                {
                    // Recargamos los datos para reflejar los cambios en la ficha
                    CargarDatos(_usuarioActual.IdUsuario);
                }
            }
        }
    }
}