using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;

namespace Vetcare.Presentacion.Clientes
{
    public partial class UC_DatosCliente : UserControl
    {
        private WindowFichaCliente _ventanaPadre;

        public UC_DatosCliente(WindowFichaCliente padre)
        {
            InitializeComponent();
            _ventanaPadre = padre;
        }

        private void btnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is Cliente cliente)
            {
                WindowCliente ventanaEditar = new WindowCliente(cliente);
                ventanaEditar.Owner = Window.GetWindow(this);
                if (ventanaEditar.ShowDialog() == true)
                {
                    // Refrescar los datos en la ventana principal
                    _ventanaPadre.CargarCliente(cliente.IdCliente);
                    this.DataContext = _ventanaPadre.DataContext;
                }
            }
        }
    }
}