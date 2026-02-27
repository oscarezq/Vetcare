using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Ventana que muestra la ficha completa de un cliente.
    /// Incluye:
    /// - Datos personales
    /// - Listado de mascotas asociadas
    /// </summary>
    public partial class WindowFichaCliente : Window
    {
        private ClienteService clienteService;
        private MascotaService mascotaService;
        private Cliente clienteActual;

        /// <summary>
        /// Constructor que recibe el ID del cliente a mostrar.
        /// </summary>
        public WindowFichaCliente(int idCliente)
        {
            InitializeComponent();

            clienteService = new ClienteService();
            mascotaService = new MascotaService();

            // Cargar cliente y mascotas
            CargarCliente(idCliente);
        }

        /// <summary>
        /// Carga el cliente por ID y asigna DataContext para binding.
        /// También carga sus mascotas.
        /// </summary>
        private void CargarCliente(int idCliente)
        {
            clienteActual = clienteService.ObtenerPorId(idCliente);

            if (clienteActual != null)
            {
                // Enlazamos el cliente al XAML
                this.DataContext = clienteActual;

                // Cargar mascotas del cliente
                CargarMascotas();
            }
            else
            {
                MessageBox.Show("No se pudo cargar el cliente.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Obtiene todas las mascotas asociadas al cliente
        /// y las muestra en el DataGrid.
        /// </summary>
        private void CargarMascotas()
        {
            List<Mascota> mascotas = mascotaService.ObtenerPorCliente(clienteActual.IdCliente);
            dgMascotas.ItemsSource = mascotas;

            // Lógica de visibilidad
            if (mascotas == null || mascotas.Count == 0)
            {
                dgMascotas.Visibility = Visibility.Collapsed;
                panelSinMascotas.Visibility = Visibility.Visible;
            }
            else
            {
                dgMascotas.Visibility = Visibility.Visible;
                panelSinMascotas.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Muestra el panel de datos personales del cliente.
        /// </summary>
        private void btnDatos_Click(object sender, RoutedEventArgs e)
        {
            panelDatos.Visibility = Visibility.Visible;
            panelMascotas.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Muestra el panel de mascotas del cliente.
        /// </summary>
        private void btnMascotas_Click(object sender, RoutedEventArgs e)
        {
            panelDatos.Visibility = Visibility.Collapsed;
            panelMascotas.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Edita los datos del cliente.
        /// </summary>
        private void btnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente ventanaEditar = new WindowCliente(clienteActual);
            ventanaEditar.ShowDialog();

            // Recargar datos después de editar
            CargarCliente(clienteActual.IdCliente);
        }

        private void HyperlinkMascota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Hyperlink hyperlink &&
                    hyperlink.DataContext is Mascota mascota)
                {
                    WindowFichaMascota ficha =
                        new WindowFichaMascota(mascota.IdMascota);

                    ficha.Owner = this;
                    ficha.ShowDialog();

                    // Recargar por si hubo cambios
                    CargarMascotas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ficha de la mascota: " + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Botón "Añadir Mascota" del cliente.
        /// Bloquea el dueño para que sea siempre el cliente actual.
        /// </summary>
        private void btnAnadirMascota_Click(object sender, RoutedEventArgs e)
        {
            // Crear ventana de nueva mascota
            WindowMascota ventanaNueva = new WindowMascota();
            ventanaNueva.Owner = this;

            ventanaNueva.txtNombreDueño.Text = clienteActual.Nombre + " " + clienteActual.Apellidos + " (" + clienteActual.NumDocumento + ")";
            ventanaNueva.txtNombreDueño.IsEnabled = false;
            ventanaNueva.btnSeleccionarDueño.IsEnabled = false;

            // Asignar el dueño al cliente actual
            ventanaNueva.AsignarDueno(clienteActual);

            ventanaNueva.ShowDialog();

            // Recargar lista después de añadir
            CargarMascotas();
        }
    }
}