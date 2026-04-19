using System;
using System.Linq;
using System.Windows;
using Vetcare.Entidades;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using Vetcare.Negocio.Services;
using Vetcare.Negocio.Informes;

namespace Vetcare.Presentacion.Facturas
{
    /// <summary>
    /// Ventana que muestra el detalle completo de una factura,
    /// incluyendo líneas, totales, estado e impresión en PDF.
    /// </summary>
    public partial class WindowDetalleFactura : Window
    {
        // Factura actual mostrada en la ventana
        private readonly Factura factura;

        // Servicios de negocio
        private readonly FacturaService facturaService = new();
        private readonly ClienteService clienteService = new();

        /// <summary>
        /// Constructor de la ventana de detalle de factura
        /// </summary>
        public WindowDetalleFactura(Factura factura)
        {
            InitializeComponent();

            // Asigna la factura recibida
            this.factura = factura;

            DataContext = factura;

            // Carga los datos en pantalla
            CargarDatos(this.factura);
        }

        /// <summary>
        /// Carga todos los datos de la factura en la UI
        /// </summary>
        private void CargarDatos(Factura f)
        {
            // CÁLCULO DE TOTALES
            if (f.Detalles != null && f.Detalles.Count > 0)
            {
                dgDetalles.ItemsSource = factura.Detalles;

                lblBaseImponible.Text = factura.Detalles.Sum(d => d.Subtotal).ToString("N2") + " €";
                lblIva.Text = factura.Detalles.Sum(d => d.IvaImporte).ToString("N2") + " €";
                lblTotal.Text = factura.Detalles.Sum(d => d.TotalLinea).ToString("N2") + " €";
            }
        }

        /// <summary>
        /// Marca la factura como pagada en base de datos
        /// </summary>
        private void BtnMarcarPagada_Click(object sender, RoutedEventArgs e)
        {
            if (facturaService.ActualizarEstadoFactura(factura.IdFactura, "Pagada"))
            {
                factura.Estado = "Pagada";

                MessageBox.Show("Factura actualizada correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // FORZAR REFRESH UI
                DataContext = null;
                DataContext = factura;

                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Error al actualizar la factura en la base de datos.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Genera e imprime la factura en PDF
        /// </summary>
        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtiene cliente de la factura
                var cliente = clienteService.ObtenerPorId(factura.IdCliente);

                // Genera documento PDF
                var documento = new FacturaDocumento(this.factura, cliente);

                // Ruta temporal del PDF
                string rutaPdf = Path.Combine(Path.GetTempPath(), $"Factura_{factura.NumeroFactura}.pdf");

                // Genera el PDF
                documento.GeneratePdf(rutaPdf);

                // Abre automáticamente el PDF
                Process.Start(new ProcessStartInfo(rutaPdf) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Error al generar PDF
                MessageBox.Show("Error al generar PDF: " + ex.Message);
            }
        }
    }
}