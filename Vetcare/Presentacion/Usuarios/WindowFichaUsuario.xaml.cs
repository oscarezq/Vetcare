using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Usuarios
{
    public partial class WindowFichaUsuario : Window
    {
        private UsuarioService _usuarioService = new UsuarioService();
        private VeterinarioService _veteService = new VeterinarioService();
        private Usuario _usuarioActual;

        public WindowFichaUsuario(int idUsuario)
        {
            InitializeComponent();
            CargarDatos(idUsuario);
        }

        private void CargarDatos(int idUsuario)
        {
            _usuarioActual = _usuarioService.ObtenerPorId(idUsuario);
            if (_usuarioActual == null) { MessageBox.Show("Usuario no encontrado."); this.Close(); return; }
            this.DataContext = _usuarioActual;

            // --- LÓGICA DE RESTRICCIÓN MODIFICADA ---
            if (!_usuarioActual.Activo || (Sesion.UsuarioActual != null && Sesion.UsuarioActual.IdRol == 2))
            {
                btnEditarUsuario.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnEditarUsuario.Visibility = Visibility.Visible;
            }

            // Solo si es veterinario mostramos la sección profesional
            if (_usuarioActual.NombreRol == "Veterinario")
            {
                var vet = _veteService.ObtenerPorIdUsuario(_usuarioActual.IdUsuario);
                if (vet != null)
                {
                    seccionProfesional.Visibility = Visibility.Visible;
                    txtNumeroColegiado.Text = vet.NumeroColegiado;
                    txtEspecialidad.Text = vet.Especialidad;
                }
            }
            else
            {
                seccionProfesional.Visibility = Visibility.Collapsed;
            }
        }

        private void btnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Llamamos a tu ventana de edición existente
            WindowUsuario winEdit = new WindowUsuario(_usuarioActual);
            winEdit.Owner = this;

            if (winEdit.ShowDialog() == true)
            {
                // Si se guardaron cambios, recargamos la ficha para que se vean los datos nuevos
                CargarDatos(_usuarioActual.IdUsuario);
            }
        }
    }
}