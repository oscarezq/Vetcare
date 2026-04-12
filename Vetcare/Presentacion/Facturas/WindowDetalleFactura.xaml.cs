using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Previewer;
using QuestPDF.Companion;

namespace Vetcare.Presentacion.Facturas
{
    public partial class WindowDetalleFactura : Window
    {
        private Factura factura;
        private FacturaService facturaService = new FacturaService();
        private ClienteService clienteService = new ClienteService();

        public WindowDetalleFactura(Factura factura)
        {
            InitializeComponent();
            this.factura = factura;
            CargarDatos(this.factura);
        }

        private void CargarDatos(Factura f)
        {
            // Datos básicos
            lblNumFactura.Text = $"FACTURA: #{f.NumeroFactura}";
            txtCliente.Text = f.NombreCliente;
            txtFecha.Text = f.FechaEmision.ToString("dd/MM/yyyy");
            txtMetodoPago.Text = f.MetodoPago ?? "Efectivo";
            txtEstado.Text = f.Estado.ToUpper();
            txtObservaciones.Text = string.IsNullOrWhiteSpace(f.Observaciones) ? "Sin observaciones." : f.Observaciones;

            // Cálculos dinámicos de totales
            if (f.Detalles != null && f.Detalles.Count > 0)
            {
                decimal subtotalSinIva = f.Detalles.Sum(d => d.Subtotal);
                decimal totalIva = f.Detalles.Sum(d => d.IvaImporte);
                decimal total = f.Detalles.Sum(d => d.TotalLinea);

                lblBaseImponible.Text = subtotalSinIva.ToString("N2") + " €";
                lblIva.Text = totalIva.ToString("N2") + " €";
                lblTotal.Text = total.ToString("N2") + " €";

                dgDetalles.ItemsSource = f.Detalles;
            }

            // Gestión visual del estado
            ActualizarInterfazEstado(f.Estado);
        }

        private void ActualizarInterfazEstado(string estado)
        {
            switch (estado)
            {
                case "Pagada":
                    brdEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#27AE60"));
                    btnMarcarPagada.Visibility = Visibility.Collapsed;
                    break;
                case "Pendiente":
                    brdEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F39C12"));
                    btnMarcarPagada.Visibility = Visibility.Visible;
                    break;
                case "Anulada":
                    brdEstado.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C"));
                    btnMarcarPagada.Visibility = Visibility.Collapsed;
                    break;
                default:
                    brdEstado.Background = Brushes.Gray;
                    break;
            }
        }

        private void btnMarcarPagada_Click(object sender, RoutedEventArgs e)
        {
            if (facturaService.ActualizarEstadoFactura(factura.IdFactura, "Pagada"))
            {
                factura.Estado = "Pagada";
                MessageBox.Show("Factura actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                CargarDatos(factura);
            }
            else
            {
                MessageBox.Show("Error al actualizar la factura en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Instanciar el documento con la factura actual
                var cliente = clienteService.ObtenerPorId(factura.IdCliente);

                var documento = new FacturaDocumento(this.factura, cliente);

                // 2. Definir ruta temporal
                string rutaPdf = Path.Combine(Path.GetTempPath(), $"Factura_{factura.NumeroFactura}.pdf");

                // 3. Generar el PDF
                documento.GeneratePdf(rutaPdf);

                // 4. Abrirlo automáticamente
                Process.Start(new ProcessStartInfo(rutaPdf) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message);
            }
        }
    }
}