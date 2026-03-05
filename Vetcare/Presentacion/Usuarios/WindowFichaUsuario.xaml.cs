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

            // Mostrar botón de Datos Profesionales solo si es veterinario
            if (_usuarioActual.NombreRol == "Veterinario")
            {
                btnProfesional.Visibility = Visibility.Visible;

                var vet = _veteService.ObtenerPorIdUsuario(_usuarioActual.IdUsuario);
                if (vet != null)
                {
                    txtNumeroColegiado.Text = vet.NumeroColegiado;
                    txtEspecialidad.Text = vet.Especialidad;
                }
            }
            else
            {
                btnProfesional.Visibility = Visibility.Collapsed;
            }

            // Mostrar panel de datos generales por defecto
            panelDatosGenerales.Visibility = Visibility.Visible;
            panelDatosProfesional.Visibility = Visibility.Collapsed;
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            panelDatosGenerales.Visibility = Visibility.Visible;
            panelDatosProfesional.Visibility = Visibility.Collapsed;
        }

        private void btnProfesional_Click(object sender, RoutedEventArgs e)
        {
            panelDatosGenerales.Visibility = Visibility.Collapsed;
            panelDatosProfesional.Visibility = Visibility.Visible;
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