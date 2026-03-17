using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Lógica de interacción para PageProductos.xaml
    /// </summary>
    public partial class PageProductos : Page
    {
        // Asumo que ProductoService devuelve objetos de tipo Producto (que heredan de Concepto o son similares)
        private ConceptoService conceptoService = new ConceptoService();
        private List<Concepto> listaCompleta = new List<Concepto>();

        public PageProductos()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                // Obtenemos la lista desde la capa de negocio
                listaCompleta = conceptoService.ObtenerProductos();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltros()
        {
            // Verificación de seguridad para evitar NullReferenceException durante la inicialización
            if (listaCompleta == null || rbAsc == null || dgProductos == null) return;

            IEnumerable<Concepto> filtrados = listaCompleta;

            // 1. Filtro por Nombre
            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
            {
                filtrados = filtrados.Where(p => p.Nombre.ToLower().Contains(txtBuscaNombre.Text.ToLower()));
            }

            // 2. Filtro por Precio Mínimo
            if (decimal.TryParse(txtPrecioMin.Text, out decimal pMin))
            {
                filtrados = filtrados.Where(p => p.PrecioBase >= pMin);
            }

            // 3. Filtro por Precio Máximo
            if (decimal.TryParse(txtPrecioMax.Text, out decimal pMax))
            {
                filtrados = filtrados.Where(p => p.PrecioBase <= pMax);
            }

            // 4. Filtro por Stock Mínimo
            if (int.TryParse(txtStockMin.Text, out int sMin))
            {
                filtrados = filtrados.Where(p => p.Stock >= sMin);
            }

            // 5. Filtro por Stock Máximo
            if (int.TryParse(txtStockMax.Text, out int sMax))
            {
                filtrados = filtrados.Where(p => p.Stock <= sMax);
            }

            // 6. Ordenación
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool ascendente = rbAsc.IsChecked == true;

            switch (criterio)
            {
                case "Precio":
                    filtrados = ascendente ? filtrados.OrderBy(p => p.PrecioBase) : filtrados.OrderByDescending(p => p.PrecioBase);
                    break;
                case "Stock":
                    filtrados = ascendente ? filtrados.OrderBy(p => p.Stock) : filtrados.OrderByDescending(p => p.Stock);
                    break;
                default: // Por defecto: Nombre
                    filtrados = ascendente ? filtrados.OrderBy(p => p.Nombre) : filtrados.OrderByDescending(p => p.Nombre);
                    break;
            }

            dgProductos.ItemsSource = filtrados.ToList();
        }

        // Eventos de cambio en los filtros (unificados como Filtro_Changed en el XAML)
        private void Filtro_Changed(object sender, TextChangedEventArgs e) => AplicarFiltros();
        private void Filtro_Changed(object sender, SelectionChangedEventArgs e) => AplicarFiltros();
        private void Filtro_Changed(object sender, RoutedEventArgs e) => AplicarFiltros();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Text = "";
            txtPrecioMin.Text = "";
            txtPrecioMax.Text = "";
            txtStockMin.Text = "";
            txtStockMax.Text = "";
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            AplicarFiltros();
        }

        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            // Cambiar a tu ventana real de Producto
            WindowProducto win = new WindowProducto();
            if (win.ShowDialog() == true)
                CargarDatos();
        }

        private void btnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el producto de la fila seleccionada
            var producto = (Concepto)((Button)sender).DataContext;
            WindowProducto win = new WindowProducto(producto);
            if (win.ShowDialog() == true)
                CargarDatos();
        }

        private void btnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            var producto = (Concepto)((Button)sender).DataContext;
            var result = MessageBox.Show($"¿Estás seguro de eliminar el producto {producto.Nombre}?",
                                       "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Lógica para eliminar mediante el servicio
                // productoService.Eliminar(producto.IdProducto);
                CargarDatos();
            }
        }
    }
}