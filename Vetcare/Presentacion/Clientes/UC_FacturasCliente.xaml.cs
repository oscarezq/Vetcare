using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySqlX.XDevAPI;
using Vetcare.Datos.DAOs;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Facturas;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// UserControl encargado de mostrar y gestionar las facturas asociadas a un cliente.
    /// Permite visualizar facturas, calcular deuda y crear nuevas facturas.
    /// </summary>
    public partial class UC_FacturasCliente : UserControl
    {
        // Cliente actualmente seleccionado
        private readonly Cliente _clienteActual;

        // Servicio de negocio para operaciones con facturas
        private readonly FacturaService _facturaService = new();

        /// <summary>
        /// Constructor del control de facturas del cliente
        /// </summary>
        public UC_FacturasCliente(Cliente clienteActual)
        {
            InitializeComponent();

            // Se asigna el cliente recibido
            _clienteActual = clienteActual;

            // Carga inicial de datos
            CargarDatos();
        }

        /// <summary>
        /// Carga las facturas del cliente y calcula la deuda total
        /// </summary>
        private void CargarDatos()
        {
            // Obtiene facturas del cliente
            var facturas = _facturaService.ObtenerPorCliente(_clienteActual.IdCliente);

            // Calcula la deuda total del cliente
            decimal deudaTotal = _facturaService.CalcularDeudaCliente(_clienteActual.IdCliente);

            // Si no hay facturas, se muestra panel alternativo
            if (facturas == null || facturas.Count == 0)
            {
                dgFacturas.Visibility = Visibility.Collapsed;
                pnlSinFacturas.Visibility = Visibility.Visible;
            }
            else
            {
                // Se muestran las facturas en el DataGrid
                dgFacturas.ItemsSource = facturas;
                dgFacturas.Visibility = Visibility.Visible;
                pnlSinFacturas.Visibility = Visibility.Collapsed;
            }

            // Se muestra la deuda formateada
            lblDeuda.Text = string.Format("{0:N2} €", deudaTotal);

            // Si no hay deuda, se cambia el color a gris
            if (deudaTotal <= 0)
            {
                lblDeuda.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        /// <summary>
        /// Abre la ventana de detalle de una factura seleccionada
        /// </summary>
        private void BtnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            // Obtiene el botón pulsado

            // Verifica que tenga una factura asociada
            if (sender is Button btn && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    // DAO de detalles de factura
                    DetalleFacturaDAO detalleDAO = new();

                    // Carga los detalles de la factura
                    facturaSeleccionada.Detalles = detalleDAO
                        .ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    // Abre ventana de detalle
                    WindowDetalleFactura win = new(facturaSeleccionada)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    win.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Muestra error si falla la carga
                    MessageBox.Show("Error al cargar la factura: " + ex.Message,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ventana para crear una nueva factura del cliente
        /// </summary>
        private void BtnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            // Abre ventana de creación de factura asociada al cliente
            WindowFactura win = new(_clienteActual)
            {
                Owner = Window.GetWindow(this)
            };

            // Si se crea correctamente, se recargan datos
            if (win.ShowDialog() == true)
            {
                CargarDatos();
            }
        }
    }
}