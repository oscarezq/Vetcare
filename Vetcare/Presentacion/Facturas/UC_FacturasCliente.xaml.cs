using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Facturas;

namespace Vetcare.Presentacion.Clientes
{
    public partial class UC_FacturasCliente : UserControl
    {
        public UC_FacturasCliente(List<Factura> facturas, decimal deudaTotal)
        {
            InitializeComponent();
            CargarDatos(facturas, deudaTotal);
        }

        private void CargarDatos(List<Factura> facturas, decimal deudaTotal)
        {
            if (facturas == null || facturas.Count == 0)
            {
                dgFacturas.Visibility = Visibility.Collapsed;
                pnlSinFacturas.Visibility = Visibility.Visible;
            }
            else
            {
                dgFacturas.ItemsSource = facturas;
                dgFacturas.Visibility = Visibility.Visible;
                pnlSinFacturas.Visibility = Visibility.Collapsed;
            }

            lblDeuda.Text = string.Format("{0:N2} €", deudaTotal);

            // Si no debe nada, ocultamos el panel de deuda o cambiamos color
            if (deudaTotal <= 0)
            {
                lblDeuda.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void btnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    DetalleFacturaDAO detalleDAO = new DetalleFacturaDAO();
                    facturaSeleccionada.Detalles = detalleDAO
                        .ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    WindowDetalleFactura win = new WindowDetalleFactura(facturaSeleccionada);
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la factura: " + ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnImprimirEstado_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Generando extracto de cuenta...");
        }
    }
}