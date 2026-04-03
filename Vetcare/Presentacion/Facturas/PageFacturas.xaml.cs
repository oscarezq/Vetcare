using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Facturas
{
    public partial class PageFacturas : Page
    {
        private List<Factura> listaFacturas;
        private FacturaService facturaService = new FacturaService();

        public PageFacturas()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                listaFacturas = facturaService.ObtenerTodas();
                AplicarFiltrosYOrden();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las facturas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltrosYOrden()
        {
            if (listaFacturas == null) return;

            var filtrados = listaFacturas.AsEnumerable();

            // 1. Filtro por Texto (Cliente o Número de Factura)
            if (!string.IsNullOrWhiteSpace(txtBuscaGeneral.Text))
            {
                string busqueda = txtBuscaGeneral.Text.ToLower().Trim();
                filtrados = filtrados.Where(f =>
                    (f.NombreCliente != null && f.NombreCliente.ToLower().Contains(busqueda)) ||
                    (f.NumeroFactura != null && f.NumeroFactura.ToLower().Contains(busqueda))
                );
            }

            // 2. Filtro por Rango de Fechas
            if (dpDesde.SelectedDate.HasValue)
                filtrados = filtrados.Where(f => f.FechaEmision >= dpDesde.SelectedDate.Value);

            if (dpHasta.SelectedDate.HasValue)
                filtrados = filtrados.Where(f => f.FechaEmision <= dpHasta.SelectedDate.Value);

            // 3. Filtro por Estado (¡Añadido!)
            if (cbEstado.SelectedIndex > 0) // El índice 0 es "Todos"
            {
                string estadoSeleccionado = (cbEstado.SelectedItem as ComboBoxItem).Content.ToString();
                filtrados = filtrados.Where(f => f.Estado == estadoSeleccionado);
            }

            // 4. Lógica de Ordenación
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem).Content.ToString();
            bool descendente = rbDesc.IsChecked ?? true;

            switch (criterio)
            {
                case "Cliente":
                    filtrados = descendente ? filtrados.OrderByDescending(f => f.NombreCliente) : filtrados.OrderBy(f => f.NombreCliente);
                    break;
                case "Total":
                    filtrados = descendente ? filtrados.OrderByDescending(f => f.Total) : filtrados.OrderBy(f => f.Total);
                    break;
                default:
                    filtrados = descendente ? filtrados.OrderByDescending(f => f.FechaEmision) : filtrados.OrderBy(f => f.FechaEmision);
                    break;
            }

            dgFacturas.ItemsSource = filtrados.ToList();
        }

        private void FiltroFactura_Changed(object sender, EventArgs e) => AplicarFiltrosYOrden();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaGeneral.Clear();
            dpDesde.SelectedDate = null;
            dpHasta.SelectedDate = null;
            cbEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;
            rbDesc.IsChecked = true;
            AplicarFiltrosYOrden();
        }

        private void dgFacturas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 1. Verificar que el usuario hizo clic en una fila con datos
            if (dgFacturas.SelectedItem is Factura facturaSeleccionada)
            {
                try
                {
                    // 2. Instanciar tu nuevo DAO
                    DetalleFacturaDAO detalleDAO = new DetalleFacturaDAO();

                    // 3. Cargar las líneas de la base de datos a la factura
                    facturaSeleccionada.Detalles = detalleDAO.ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    // 4. Abrir la ventana de detalle pasando el objeto ya "relleno"
                    WindowDetalleFactura detalleWin = new WindowDetalleFactura(facturaSeleccionada);
                    detalleWin.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los detalles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            WindowFactura NuevaFacturaWin = new WindowFactura();
            if (NuevaFacturaWin.ShowDialog() == true) 
                CargarDatos();
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtenemos el botón que disparó el evento
            Button btn = sender as Button;

            // 2. El DataContext de ese botón es automáticamente el objeto 'Factura' de esa fila
            if (btn != null && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    // 3. Cargamos los detalles (Igual que haces en el DoubleClick)
                    DetalleFacturaDAO detalleDAO = new DetalleFacturaDAO();
                    facturaSeleccionada.Detalles = detalleDAO.ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    // 4. Abrimos la ventana
                    WindowDetalleFactura detalleWin = new WindowDetalleFactura(facturaSeleccionada);
                    detalleWin.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los detalles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Factura factura)
            {
                MessageBox.Show($"Generando PDF para la factura {factura.NumeroFactura}...", "Imprimir", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnAnular_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Factura factura)
            {
                if (factura.Estado == "Anulada")
                {
                    MessageBox.Show("Esta factura ya está anulada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show($"¿Está seguro de que desea anular la factura {factura.NumeroFactura}?",
                    "Confirmar Anulación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Usamos IdFactura que es como se llama en tu DAO
                    if (facturaService.AnularFactura(factura.IdFactura))
                    {
                        CargarDatos();
                    }
                }
            }
        }
    }
}