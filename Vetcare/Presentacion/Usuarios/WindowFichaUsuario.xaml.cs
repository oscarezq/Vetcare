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

            // Solo si es veterinario mostramos la sección y cargamos sus datos
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

            panelDatosGenerales.Visibility = Visibility.Visible;
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            panelDatosGenerales.Visibility = Visibility.Visible;
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