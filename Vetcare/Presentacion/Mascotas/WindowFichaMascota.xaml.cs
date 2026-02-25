using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Ventana que muestra la ficha completa de una mascota.
    /// Contiene un menú lateral con:
    /// - Datos de la mascota
    /// - Historial clínico
    /// </summary>
    public partial class WindowFichaMascota : Window
    {
        // Servicio para acceder a la lógica de negocio de Mascotas
        private MascotaService mascotaService = new MascotaService();

        // Mascota actualmente cargada en la ficha
        private Mascota mascotaActual;

        /// <summary>
        /// Constructor que recibe el ID de la mascota seleccionada
        /// </summary>
        /// <param name="idMascota">ID de la mascota</param>
        public WindowFichaMascota(int idMascota)
        {
            InitializeComponent();

            // Obtener la mascota desde la base de datos
            mascotaActual = mascotaService.ObtenerPorId(idMascota);

            if (mascotaActual != null)
            {
                // Asignar la mascota como DataContext para que el XAML
                // pueda enlazar automáticamente las propiedades
                this.DataContext = mascotaActual;
            }
            else
            {
                MessageBox.Show("No se pudo cargar la mascota.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                this.Close();
            }
        }

        /// <summary>
        /// Muestra el panel de Datos de la Mascota
        /// y oculta el panel de Historial.
        /// </summary>
        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            panelDatos.Visibility = Visibility.Visible;
            panelHistorial.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Muestra el panel de Historial Clínico
        /// y oculta el panel de Datos.
        /// </summary>
        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            panelDatos.Visibility = Visibility.Collapsed;
            panelHistorial.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Abre la ventana de edición de la mascota actual.
        /// Después de cerrar la ventana, recarga los datos
        /// para reflejar posibles cambios.
        /// </summary>
        private void btnEditarMascota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Abrir ventana en modo edición pasando la mascota actual
                WindowMascota ventanaEditar = new WindowMascota(mascotaActual);

                // Bloquear el dueño: no dejar cambiarlo
                ventanaEditar.txtNombreDueño.Text = mascotaActual.Dueno;
                ventanaEditar.txtNombreDueño.IsEnabled = false;
                ventanaEditar.btnSeleccionarDueño.IsEnabled = false;

                ventanaEditar.Owner = this;
                ventanaEditar.ShowDialog();

                // Recargar la mascota desde la base de datos
                mascotaActual = mascotaService.ObtenerPorId(mascotaActual.IdMascota);

                // Actualizar el DataContext para refrescar la vista
                this.DataContext = mascotaActual;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar mascota: " + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void HyperlinkDueno_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mascotaActual != null && mascotaActual.IdCliente > 0)
                {
                    // Abrir ventana de datos del dueño
                    WindowFichaCliente ventanaDueno = new WindowFichaCliente(mascotaActual.IdCliente);
                    ventanaDueno.Owner = this;
                    ventanaDueno.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ficha del dueño: " + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}