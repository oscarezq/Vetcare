using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    public partial class WindowFichaCliente : Window
    {
        private ClienteService clienteService;
        private Cliente clienteActual;

        public WindowFichaCliente(int idCliente)
        {
            InitializeComponent();
            clienteService = new ClienteService();
            CargarCliente(idCliente);

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
            // Inyectamos el cliente actual al UserControl de mascotas
            ContenedorPrincipal.Content = new UC_MascotasCliente(clienteActual);
        }

        private void MostrarDatosPersonales()
        {
            // Inyectamos la ventana padre (this) para que el UC pueda llamar a CargarCliente si edita datos
            ContenedorPrincipal.Content = new UC_DatosCliente(this) { DataContext = clienteActual };
        }
    }
}