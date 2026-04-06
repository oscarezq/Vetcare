using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySqlX.XDevAPI;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Facturas;

namespace Vetcare.Presentacion.Clientes
{
    public partial class UC_FacturasCliente : UserControl
    {
        private Cliente _clienteActual;
        private FacturaService _facturaService = new FacturaService();

        public UC_FacturasCliente(Cliente clienteActual)
        {
            InitializeComponent();
            _clienteActual = clienteActual;

            CargarDatos();
        }

        private void CargarDatos()
        {
            var facturas = _facturaService.ObtenerPorCliente(_clienteActual.IdCliente);
            decimal deudaTotal = _facturaService.CalcularDeudaCliente(_clienteActual.IdCliente);

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

        private void btnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            // Aquí abrimos la ventana de creación de factura
            // Pasamos el cliente para que la factura ya esté asociada a él
            WindowFactura win = new WindowFactura(_clienteActual);
            win.Owner = Window.GetWindow(this);

            if (win.ShowDialog() == true)
            {
                CargarDatos();
            }
        }

    }
}