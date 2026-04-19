using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Ventana principal que muestra la ficha completa de un cliente,
    /// incluyendo datos personales, mascotas y facturación.
    /// </summary>
    public partial class WindowFichaCliente : Window
    {
        // Servicio para operaciones relacionadas con clientes
        private readonly ClienteService clienteService;

        // Cliente actualmente cargado en la ventana
        private Cliente? clienteActual;

        /// <summary>
        /// Constructor de la ventana.
        /// Recibe el ID del cliente a cargar.
        /// </summary>
        public WindowFichaCliente(int idCliente)
        {
            InitializeComponent();

            // Inicialización de servicios
            clienteService = new ClienteService();

            // Carga del cliente
            CargarCliente(idCliente);

            // Mostrar por defecto los datos personales al abrir la ventana
            MostrarDatosPersonales();
        }

        /// <summary>
        /// Carga los datos del cliente desde la base de datos
        /// y los asigna al DataContext para el binding.
        /// </summary>
        public void CargarCliente(int idCliente)
        {
            clienteActual = clienteService.ObtenerPorId(idCliente);

            if (clienteActual != null)
            {
                // Asignación del DataContext para enlazar con la UI
                this.DataContext = clienteActual;
            }
            else
            {
                // Mensaje de error si no se encuentra el cliente
                MessageBox.Show("No se pudo cargar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Muestra los datos personales del cliente en el contenedor principal.
        /// </summary>
        private void MostrarDatosPersonales()
        {
            if (clienteActual != null)
            {
                var uc = new UC_DatosCliente(this);

                // Ocultar botón de editar si el cliente está inactivo
                if (!clienteActual.Activo)
                    uc.btnEditarCliente.Visibility = Visibility.Collapsed;

                // Cargar el UserControl en el contenedor principal
                ContenedorPrincipal.Content = uc;
            }
        }

        /// <summary>
        /// Evento click del botón "Datos Personales".
        /// </summary>
        private void BtnDatos_Click(object sender, RoutedEventArgs e) => MostrarDatosPersonales();

        /// <summary>
        /// Evento click del botón "Mascotas".
        /// Carga el UserControl de mascotas del cliente.
        /// </summary>
        private void BtnMascotas_Click(object sender, RoutedEventArgs e)
        {
            if (clienteActual != null)
            {
                var uc = new UC_MascotasCliente(clienteActual);

                // Ocultar botón de añadir mascota si el cliente está inactivo
                if (!clienteActual.Activo)
                    uc.btnAnadirMascota.Visibility = Visibility.Collapsed;

                // Cargar el UserControl en el contenedor principal
                ContenedorPrincipal.Content = uc;
            }
        }

        /// <summary>
        /// Evento click del botón "Facturación".
        /// Carga el UserControl de facturas del cliente.
        /// </summary>
        private void BtnFacturas_Click(object sender, RoutedEventArgs e)
        {
            if (clienteActual != null)
            {
                // Crear el UserControl pasándole el cliente actual
                var uc = new UC_FacturasCliente(clienteActual);

                // Inyectar el UserControl en el contenedor principal
                ContenedorPrincipal.Content = uc;
            }
        }
    }
}