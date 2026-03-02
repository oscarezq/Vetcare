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
            try
            {
                // 1. Obtener los datos base del usuario
                _usuarioActual = _usuarioService.ObtenerPorId(idUsuario);

                if (_usuarioActual == null)
                {
                    MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                    return;
                }

                // 2. Vincular a la interfaz
                this.DataContext = _usuarioActual;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la ficha: {ex.Message}", "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            // En este caso solo tenemos un panel, pero lo dejamos listo para futuros paneles (ej. Horarios)
            panelDatos.Visibility = Visibility.Visible;
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