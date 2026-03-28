using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion.Facturas
{
    public partial class WindowFactura : Window
    {
        private ObservableCollection<DetalleFactura> listaDetalles = new ObservableCollection<DetalleFactura>();
        private FacturaService facturaService = new FacturaService();
        private Concepto conceptoSeleccionado;

        public WindowFactura()
        {
            InitializeComponent();
            dgDetalles.ItemsSource = listaDetalles;
            // Inicializamos el número de factura al abrir la ventana
            txtNumFactura.Text = ObtenerSiguienteNumeroFactura();
            lblTotal.Text = "0,00 €";
        }

        public string ObtenerSiguienteNumeroFactura()
        {
            int anioActual = DateTime.Now.Year;
            // Importante: Asegúrate que facturaService.ObtenerUltimoNumeroPorAnio llame al DAO que ya corregimos
            string ultimoNumero = facturaService.ObtenerUltimoNumeroPorAnio(anioActual);

            if (string.IsNullOrEmpty(ultimoNumero))
                return $"{anioActual}-0001";

            try
            {
                // El formato esperado es "2026-0005"
                string[] partes = ultimoNumero.Split('-');
                if (partes.Length == 2 && int.TryParse(partes[1], out int contador))
                {
                    contador++;
                    return $"{anioActual}-{contador:D4}"; // El :D4 rellena con ceros a la izquierda
                }
            }
            catch { }

            return $"{anioActual}-0001";
        }

        // --- SELECTORES ---
        private void btnSeleccionarCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorCliente selector = new WindowSelectorCliente { Owner = this };
            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;
                txtIdCliente.Text = cliente.IdCliente.ToString();
                txtNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellidos}";
                txtNombreCliente.Foreground = Brushes.Black;
                txtNombreCliente.FontWeight = FontWeights.Bold;
            }
        }

        private void btnSeleccionarServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorConcepto selector = new WindowSelectorConcepto { Owner = this };
            if (selector.ShowDialog() == true)
            {
                conceptoSeleccionado = selector.ConceptoSeleccionado;
                txtNombreServicio.Text = $"{conceptoSeleccionado.Nombre} ({conceptoSeleccionado.Precio:N2}€)";
                txtNombreServicio.Foreground = Brushes.Black;
                txtNombreServicio.FontWeight = FontWeights.Bold;
            }
        }

        // --- GESTIÓN DE LA LISTA (GRID) ---
        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (conceptoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto o servicio.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int.TryParse(txtCantidad.Text, out int cantidad);
            if (cantidad <= 0) cantidad = 1;

            if (conceptoSeleccionado.Tipo == "Producto" && conceptoSeleccionado.Stock < cantidad)
            {
                MessageBox.Show($"Stock insuficiente. Disponible: {conceptoSeleccionado.Stock}",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Buscamos si ya está en la lista para no repetir filas
            var existente = listaDetalles.FirstOrDefault(d => d.IdConcepto == conceptoSeleccionado.IdConcepto
                                                           && d.Tipo == conceptoSeleccionado.Tipo);

            if (existente != null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                listaDetalles.Add(new DetalleFactura
                {
                    IdConcepto = conceptoSeleccionado.IdConcepto,
                    NombreConcepto = conceptoSeleccionado.Nombre,
                    Tipo = conceptoSeleccionado.Tipo,
                    Cantidad = cantidad,
                    PrecioUnitario = conceptoSeleccionado.Precio,
                    IvaPorcentaje = conceptoSeleccionado.IvaPorcentaje
                });
            }

            ActualizarInterfaz();
            LimpiarSelectorConcepto();
        }

        private void btnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is DetalleFactura detalle)
            {
                listaDetalles.Remove(detalle);
                ActualizarInterfaz();
            }
        }

        private void btnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is DetalleFactura detalle)
            {
                detalle.Cantidad++;
                ActualizarInterfaz();
            }
        }

        private void btnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is DetalleFactura detalle && detalle.Cantidad > 1)
            {
                detalle.Cantidad--;
                ActualizarInterfaz();
            }
        }

        // --- GUARDADO ---
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdCliente.Text) || listaDetalles.Count == 0)
            {
                MessageBox.Show("Faltan datos (Cliente o Conceptos).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("¿Generar factura?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Factura nuevaFactura = new Factura
                    {
                        NumeroFactura = txtNumFactura.Text,
                        IdCliente = int.Parse(txtIdCliente.Text),
                        MetodoPago = (cbMetodoPago.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Efectivo",
                        Observaciones = txtObservaciones.Text,
                        Detalles = listaDetalles.ToList() // Pasamos la lista al objeto factura
                    };

                    if (facturaService.InsertarFacturaCompleta(nuevaFactura))
                    {
                        MessageBox.Show("Factura guardada correctamente.", "Éxito");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo guardar: " + ex.Message);
                }
            }
        }

        // --- CÁLCULOS ---
        private void ActualizarInterfaz()
        {
            // Forzamos al DataGrid a redibujarse para ver los cambios de cantidad
            dgDetalles.Items.Refresh();

            decimal baseImp = listaDetalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            decimal ivaTotal = listaDetalles.Sum(d => (d.Cantidad * d.PrecioUnitario) * (d.IvaPorcentaje / 100));
            decimal total = baseImp + ivaTotal;

            lblBaseImponible.Text = $"{baseImp:N2} €";
            lblIvaTotal.Text = $"{ivaTotal:N2} €";
            lblTotal.Text = $"{total:N2} €";
        }

        private void LimpiarSelectorConcepto()
        {
            conceptoSeleccionado = null;
            txtNombreServicio.Text = "Seleccionar concepto...";
            txtNombreServicio.Foreground = Brushes.Gray;
            txtNombreServicio.FontWeight = FontWeights.Normal;
            txtCantidad.Text = "1";
        }

        // Botones de + y - del selector superior
        private void btnCantidadMas_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int c)) txtCantidad.Text = (c + 1).ToString();
        }

        private void btnCantidadMenos_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int c) && c > 1) txtCantidad.Text = (c - 1).ToString();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}