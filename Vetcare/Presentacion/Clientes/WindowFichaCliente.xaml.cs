using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    public partial class WindowFichaCliente : Window
    {
        private ClienteService clienteService;
        private FacturaService facturaService;
        private Cliente clienteActual;

        public WindowFichaCliente(int idCliente)
        {
            InitializeComponent();
            clienteService = new ClienteService();
            facturaService = new FacturaService();
            CargarCliente(idCliente);

            if (Sesion.UsuarioActual != null && Sesion.UsuarioActual.IdRol == 2)
            {
                // Ocultamos el botón completamente para que no pueda hacer clic
                btnFacturacionMenu.Visibility = Visibility.Collapsed;
            }

            // Mostrar por defecto los datos personales al abrir
            MostrarDatosPersonales();
        }

        public void CargarCliente(int idCliente)
        {
            clienteActual = clienteService.ObtenerPorId(idCliente);

            if (clienteActual != null)
            {
                this.DataContext = clienteActual;
            }
            else
            {
                MessageBox.Show("No se pudo cargar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e) => MostrarDatosPersonales();

        private void btnMascotas_Click(object sender, RoutedEventArgs e)
        {
            var uc = new UC_MascotasCliente(clienteActual);

            // Ocultar botón de editar si la mascota está inactiva
            if (!clienteActual.Activo)
                uc.btnAnadirMascota.Visibility = Visibility.Collapsed;

            ContenedorPrincipal.Content = uc;
        }

        private void MostrarDatosPersonales()
        {
            var uc = new UC_DatosCliente(this);

            // Ocultar botón de editar si la mascota está inactiva
            if (!clienteActual.Activo)
                uc.btnEditarCliente.Visibility = Visibility.Collapsed;

            ContenedorPrincipal.Content = uc;
        }

        private void btnFacturas_Click(object sender, RoutedEventArgs e)
        {
            // 2. Crear el UC pasándole los datos
            var uc = new UC_FacturasCliente(clienteActual);

            // 3. Inyectar en el contenedor principal
            ContenedorPrincipal.Content = uc;
        }
    }
}