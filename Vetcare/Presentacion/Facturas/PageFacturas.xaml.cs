using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Facturas
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado y operaciones
    /// sobre las facturas del sistema.
    /// Permite listar, filtrar, ordenar, crear, ver detalles, imprimir y anular facturas.
    /// </summary>
    public partial class PageFacturas : Page
    {
        // Lista completa de facturas cargadas desde la base de datos.
        private List<Factura> listaCompleta = new();

        // Servicio de negocio para la gestión de facturas.
        private readonly FacturaService cs = new();

        /// <summary>
        /// Constructor de la página de facturas.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageFacturas()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todas las facturas desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerTodas();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Actualiza la tabla aplicando filtros, búsquedas y ordenación.
        /// </summary>
        private void ActualizarTabla()
        {
            // Verificación para evitar errores durante la inicialización de componentes
            if (listaCompleta == null || dgFacturas == null)
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por cliente
            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
                filtrado = filtrado.Where(f =>
                    f.NombreApellidosCliente != null &&
                    f.NombreApellidosCliente.ToLower().Contains(txtBuscaCliente.Text.ToLower()));

            // Filtro por número de factura
            if (!string.IsNullOrWhiteSpace(txtBuscaNumero.Text))
                filtrado = filtrado.Where(f =>
                    f.NumeroFactura != null &&
                    f.NumeroFactura.ToLower().Contains(txtBuscaNumero.Text.ToLower()));

            // Filtro por fecha (desde)
            if (dpDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(f => f.FechaEmision >= dpDesde.SelectedDate.Value);

            // Filtro por fecha (hasta)
            if (dpHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(f => f.FechaEmision <= dpHasta.SelectedDate.Value);

            // Filtro por estado de la factura
            if (cbEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
                filtrado = filtrado.Where(f => f.Estado == item.Content.ToString());

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Fecha";
            bool asc = rbAsc.IsChecked == true;

            filtrado = criterio switch
            {
                "Cliente" => asc ? filtrado.OrderBy(f => f.NombreApellidosCliente) : filtrado.OrderByDescending(f => f.NombreApellidosCliente),
                "Total" => asc ? filtrado.OrderBy(f => f.Total) : filtrado.OrderByDescending(f => f.Total),
                _ => asc ? filtrado.OrderBy(f => f.FechaEmision) : filtrado.OrderByDescending(f => f.FechaEmision),
            };

            dgFacturas.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro.
        /// </summary>
        private void FiltroFactura_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados en la vista.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaCliente.Clear();
            txtBuscaNumero.Clear();

            dpDesde.SelectedDate = null;
            dpHasta.SelectedDate = null;

            cbEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;

            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva factura.
        /// </summary>
        private void BtnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            WindowFactura ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana de detalles de una factura.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Factura f)
                AbrirVentanaDetalles(f);
        }

        /// <summary>
        /// Abre la ficha de la factura al hacer doble clic en la tabla.
        /// </summary>
        private void DgFacturas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgFacturas.SelectedItem is Factura f)
                AbrirVentanaDetalles(f);
        }

        /// <summary>
        /// Abre la ventana de ficha de factura.
        /// </summary>
        private void AbrirVentanaDetalles(Factura factura)
        {
            try
            {
                DetalleFacturaDAO detalleDAO = new();
                factura.Detalles = detalleDAO.ObtenerDetallesPorFactura(factura.IdFactura);

                WindowDetalleFactura ventana = new(factura)
                {
                    Owner = Window.GetWindow(this)
                };

                if (ventana.ShowDialog() == true)
                    CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los detalles: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Genera el documento de la factura (simulación).
        /// </summary>
        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Factura f)
            {
                MessageBox.Show($"Generando PDF para la factura {f.NumeroFactura}...",
                    "Imprimir",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Anula una factura tras confirmación del usuario.
        /// </summary>
        private void BtnAnular_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Factura f)
            {
                if (f.Estado == "Anulada")
                {
                    MessageBox.Show("Esta factura ya está anulada.",
                        "Aviso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"¿Está seguro de que desea anular la factura {f.NumeroFactura}?",
                    "Confirmar Anulación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (cs.AnularFactura(f.IdFactura))
                        CargarDatos();
                }
            }
        }
    }
}