using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

        // ================= ACCIONES DE LA TABLA =================

        /// <summary>
        /// Botón "Ver ficha" de la mascota.
        /// Abre la ventana de ficha de la mascota seleccionada.
        /// </summary>
        private void VerFichaMascota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is Mascota mascota)
            {
                WindowFichaMascota ficha = new WindowFichaMascota(mascota.IdMascota);
                ficha.Owner = this;
                ficha.ShowDialog();

                // Recargar lista en caso de cambios
                CargarMascotas();
            }
        }

        /// <summary>
        /// Botón "Editar" de la mascota.
        /// Abre la ventana de edición de la mascota bloqueando el dueño.
        /// </summary>
        private void EditarMascotaFila_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is Mascota mascota)
            {
                // Abrimos la ventana de edición pasando la mascota
                WindowMascota ventanaEditar = new WindowMascota(mascota);
                ventanaEditar.Owner = this;

                // Bloquear el dueño: no dejar cambiarlo
                ventanaEditar.txtNombreDueño.Text = clienteActual.ClienteCompleto;
                ventanaEditar.txtNombreDueño.IsEnabled = false;
                ventanaEditar.btnSeleccionarDueño.IsEnabled = false;

                ventanaEditar.ShowDialog();

                // Recargar la lista de mascotas
                CargarMascotas();
            }
        }

        /// <summary>
        /// Botón "Eliminar" de la mascota.
        /// Confirma y elimina la mascota seleccionada.
        /// </summary>
        private void EliminarMascotaFila_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is Mascota mascota)
            {
                var resultado = MessageBox.Show(
                    $"¿Seguro que quieres eliminar a {mascota.Nombre}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    if (mascotaService.Eliminar(mascota.IdMascota))
                    {
                        MessageBox.Show("Mascota eliminada correctamente.",
                                        "Información",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                        CargarMascotas();
                    }
                }
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