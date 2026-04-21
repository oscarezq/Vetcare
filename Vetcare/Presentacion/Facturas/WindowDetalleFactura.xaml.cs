using System;
using System.Linq;
using System.Windows;
using Vetcare.Entidades;
using System.Diagnostics;
using System.IO;
using QuestPDF.Fluent;
using Vetcare.Negocio.Services;
using Vetcare.Negocio.Informes;
using Microsoft.Win32;

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
                // Obtener cliente de la factura
                var cliente = clienteService.ObtenerPorId(factura.IdCliente);

                if (cliente == null)
                {
                    MessageBox.Show("No se pudo obtener el cliente.", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Diálogo para elegir dónde guardar el PDF
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Factura_{factura.NumeroFactura}_{DateTime.Now:yyyyMMdd}.pdf",
                    Title = "Guardar Factura"
                };

                // Si el usuario confirma ubicación
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Generar documento PDF
                    var documento = new FacturaDocumento(factura, cliente);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("Factura generada correctamente.",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Abrir automáticamente el PDF
                    Process.Start(new ProcessStartInfo(saveFileDialog.FileName)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}