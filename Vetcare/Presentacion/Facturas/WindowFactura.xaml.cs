using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Servicios;

namespace Vetcare.Presentacion.Facturas
{
    /// <summary>
    /// Lógica de interacción para WindowFactura.xaml
    /// </summary>
    public partial class WindowFactura : Window
    {
        private ObservableCollection<DetalleFactura> listaDetalles = new ObservableCollection<DetalleFactura>();
        private FacturaService facturaService = new FacturaService();
        private Servicio servicioSeleccionado; // Para guardar temporalmente el servicio del selector

        public WindowFactura()
        {
            InitializeComponent();
            dgDetalles.ItemsSource = listaDetalles;
            txtNumFactura.Text = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmm");
        }

        // --- SELECTOR DE CLIENTE ---
        private void btnSeleccionarCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorCliente selector = new WindowSelectorCliente();
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;
                txtIdCliente.Text = cliente.IdCliente.ToString();
                txtNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellidos}";
                txtNombreCliente.FontWeight = FontWeights.Bold;
                txtNombreCliente.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        // --- SELECTOR DE SERVICIO ---
        private void btnSeleccionarServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorServicio selector = new WindowSelectorServicio();
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                servicioSeleccionado = selector.ServicioSeleccionado;
                txtNombreServicio.Text = servicioSeleccionado.IdServicio.ToString();
                txtNombreServicio.Text = $"{servicioSeleccionado.Nombre} ({servicioSeleccionado.PrecioBase:N2}€)";
                txtNombreServicio.FontWeight = FontWeights.Bold;
                txtNombreServicio.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (servicioSeleccionado == null) return;

            int.TryParse(txtCantidad.Text, out int cantEntrada);
            int cantidadAAñadir = cantEntrada > 0 ? cantEntrada : 1;

            // BUSCAR SI YA EXISTE EL SERVICIO EN LA TABLA
            var detalleExistente = listaDetalles.FirstOrDefault(d => d.IdServicio == servicioSeleccionado.IdServicio);

            if (detalleExistente != null)
            {
                // Si existe, simplemente sumamos la cantidad
                detalleExistente.Cantidad += cantidadAAñadir;
            }
            else
            {
                // Si no existe, creamos la fila nueva
                var nuevoDetalle = new DetalleFactura
                {
                    IdServicio = servicioSeleccionado.IdServicio,
                    NombreServicio = servicioSeleccionado.Nombre,
                    Cantidad = cantidadAAñadir,
                    PrecioUnitario = servicioSeleccionado.PrecioBase
                };
                listaDetalles.Add(nuevoDetalle);
            }

            // Refrescar UI y limpiar selector
            dgDetalles.Items.Refresh();
            CalcularTotal();
            LimpiarSelectorServicio();
        }

        private void LimpiarSelectorServicio()
        {
            servicioSeleccionado = null;
            txtNombreServicio.Text = "Seleccionar servicio...";
            txtNombreServicio.Foreground = System.Windows.Media.Brushes.Gray;
            txtNombreServicio.FontWeight = FontWeights.Normal;
            txtCantidad.Text = "1";
        }

        private void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is DetalleFactura det)
            {
                listaDetalles.Remove(det);
                CalcularTotal();
            }
        }

        private void CalcularTotal()
        {
            decimal total = listaDetalles.Sum(d => d.Subtotal);
            lblTotal.Text = total.ToString("N2") + " €";
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validaciones previas
            if (string.IsNullOrEmpty(txtIdCliente.Text))
            {
                MessageBox.Show("Por favor, seleccione un cliente antes de guardar.", "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (listaDetalles.Count == 0)
            {
                MessageBox.Show("La factura debe tener al menos un servicio o producto.", "Error de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Confirmación del usuario
            var resultado = MessageBox.Show("¿Está seguro de que desea generar esta factura?", "Confirmar Guardado", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // 3. Crear el objeto Factura (Cabecera)
                    Factura nuevaFactura = new Factura
                    {
                        NumeroFactura = txtNumFactura.Text,
                        IdCliente = int.Parse(txtIdCliente.Text),
                        FechaEmision = DateTime.Now,
                        MetodoPago = (cbMetodoPago.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        Observaciones = txtObservaciones.Text,
                        Total = listaDetalles.Sum(d => d.Subtotal),
                        // Pasamos la lista de detalles
                        Detalles = listaDetalles.ToList()
                    };

                    // 4. Llamar al servicio de negocio para persistir en BD
                    bool guardadoExitoso = facturaService.InsertarFacturaCompleta(nuevaFactura);

                    if (guardadoExitoso)
                    {
                        MessageBox.Show($"Factura {nuevaFactura.NumeroFactura} guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        // 5. Opcional: ¿Quieres imprimir o limpiar la ventana?
                        LimpiarFormularioTodo();
                        this.DialogResult = true; // Cierra la ventana devolviendo éxito
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo guardar la factura en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Método auxiliar para resetear todo tras el guardado
        private void LimpiarFormularioTodo()
        {
            listaDetalles.Clear();
            txtIdCliente.Clear();
            txtNombreCliente.Text = "Seleccionar cliente...";
            txtNombreCliente.Foreground = Brushes.Gray;
            txtObservaciones.Clear();
            lblTotal.Text = "0,00 €";
            // Generar nuevo número para la siguiente (opcional si no cierras la ventana)
            txtNumFactura.Text = "FAC-" + DateTime.Now.ToString("yyyyMMddHHmm");
        }

        private void btnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is DetalleFactura detalle)
            {
                detalle.Cantidad++;
                dgDetalles.Items.Refresh();
                CalcularTotal();
            }
        }

        private void btnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is DetalleFactura detalle && detalle.Cantidad > 1)
            {
                detalle.Cantidad--;
                dgDetalles.Items.Refresh();
                CalcularTotal();
            }
        }

        private void btnCantidadMas_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = int.Parse(txtCantidad.Text);
            cantidad++;
            txtCantidad.Text = cantidad.ToString();
        }

        private void btnCantidadMenos_Click(object sender, RoutedEventArgs e)
        {
            int cantidad = int.Parse(txtCantidad.Text);

            if (cantidad > 1)
                cantidad--;

            txtCantidad.Text = cantidad.ToString();
        }

        private void ActualizarTabla()
        {
            dgDetalles.Items.Refresh(); // Fuerza a la tabla a redibujar los cambios de cantidad y subtotal
            CalcularTotal();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
