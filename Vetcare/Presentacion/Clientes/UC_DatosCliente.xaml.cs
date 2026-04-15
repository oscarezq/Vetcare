using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// UserControl que muestra los datos detallados de un cliente.
    /// Se utiliza dentro de la ventana ficha del cliente.
    /// </summary>
    public partial class UC_DatosCliente : UserControl
    {
        // Referencia a la ventana padre (ficha del cliente)
        // Se usa para poder recargar datos tras una edición
        private readonly WindowFichaCliente _fichaCliente;

        /// <summary>
        /// Constructor del UserControl.
        /// Recibe la ventana padre para poder comunicarse con ella.
        /// </summary>
        public UC_DatosCliente(WindowFichaCliente padre)
        {
            InitializeComponent();

            // Guardamos referencia a la ventana contenedora
            _fichaCliente = padre;
        }

        /// <summary>
        /// Evento del botón "Editar cliente".
        /// Abre la ventana de edición y refresca los datos si se guardan cambios.
        /// </summary>
        private void BtnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is Cliente cliente)
            {
                WindowCliente ventanaEditar = new(cliente)
                {
                    Owner = Window.GetWindow(this)
                };

                if (ventanaEditar.ShowDialog() == true)
                {
                    // Recargamos los datos desde la ventana padre
                    _fichaCliente.CargarCliente(cliente.IdCliente);

                    // Actualizamos el DataContext del UserControl
                    this.DataContext = _fichaCliente.DataContext;
                }
            }
        }
    }
}