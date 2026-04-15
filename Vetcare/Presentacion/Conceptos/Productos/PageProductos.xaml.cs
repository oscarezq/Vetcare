using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Conceptos.Productos;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado y operaciones
    /// sobre los productos del sistema.
    /// Permite listar, filtrar, ordenar, crear, editar, eliminar, reactivar y ver detalles de productos.
    /// </summary>
    public partial class PageProductos : Page
    {
        // Lista completa de productos cargados desde la base de datos.
        private List<Concepto> listaCompleta = new();

        // Servicio de negocio para la gestión de productos (conceptos).
        private readonly ConceptoService cs = new();

        /// <summary>
        /// Constructor de la página de productos.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageProductos()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los productos desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerProductos();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aplica los filtros, búsquedas y ordenación sobre la lista de productos.
        /// </summary>
        private void ActualizarTabla()
        {
            // Verificación para evitar errores durante la inicialización de componentes
            if (listaCompleta == null || rbAsc == null || dgProductos == null) 
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
                filtrado = filtrado.Where(p => p.Nombre!.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

            // Filtro por precio mínimo
            if (decimal.TryParse(txtPrecioMin.Text, out decimal pMin))
                filtrado = filtrado.Where(p => p.Precio >= pMin);

            // Filtro por precio máximo
            if (decimal.TryParse(txtPrecioMax.Text, out decimal pMax))
                filtrado = filtrado.Where(p => p.Precio <= pMax);

            // Filtro por stock mínimo
            if (int.TryParse(txtStockMin.Text, out int sMin))
                filtrado = filtrado.Where(p => p.Stock >= sMin);

            // Filtro por stock máximo
            if (int.TryParse(txtStockMax.Text, out int sMax))
                filtrado = filtrado.Where(p => p.Stock <= sMax);

            // Filtro por estado (Activo / Inactivo / Todos)
            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
            {
                if (item.Content.ToString() == "Activo")
                    filtrado = filtrado.Where(c => c.Activo);
                else if (item.Content.ToString() == "Inactivo")
                    filtrado = filtrado.Where(c => !c.Activo);
            }

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Nombre";
            bool ascendente = rbAsc.IsChecked == true;

            filtrado = criterio switch
            {
                "Precio" => ascendente ? filtrado.OrderBy(p => p.Precio) : filtrado.OrderByDescending(p => p.Precio),
                "Stock" => ascendente ? filtrado.OrderBy(p => p.Stock) : filtrado.OrderByDescending(p => p.Stock),
                _ => ascendente ? filtrado.OrderBy(p => p.Nombre) : filtrado.OrderByDescending(p => p.Nombre),
            };

            dgProductos.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro de texto.
        /// </summary>
        private void Filtro_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados en la vista.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Text = "";
            txtPrecioMin.Text = "";
            txtPrecioMax.Text = "";
            txtStockMin.Text = "";
            txtStockMax.Text = "";

            cbOrdenarPor.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;

            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo producto.
        /// </summary>
        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            WindowProducto ventana = new() 
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar un producto existente.
        /// </summary>
        private void BtnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            var producto = (Concepto)((Button)sender).DataContext;

            WindowProducto ventana = new(producto)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Elimina un producto tras confirmación del usuario.
        /// </summary>
        private void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            var producto = (Concepto)((Button)sender).DataContext;

            var result = MessageBox.Show(
                $"¿Estás seguro de eliminar el producto {producto.Nombre}?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                cs.Eliminar(producto.IdConcepto);
                CargarDatos();
            }
        }

        /// <summary>
        /// Abre la ficha del producto al hacer doble clic en la tabla.
        /// </summary>
        private void DgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgProductos.SelectedItem is Concepto productoSeleccionado)
            {
                try
                {
                    AbrirVentanaDetalles(productoSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar abrir el detalle del producto: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ventana de detalles del producto seleccionado.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is Concepto productoSeleccionado)
            {
                try
                {
                    AbrirVentanaDetalles(productoSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al intentar abrir el detalle del producto: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ventana de ficha de producto.
        /// </summary>
        /// <param name="idProducto">ID del producto a mostrar.</param>
        private void AbrirVentanaDetalles(int idProducto)
        {
            WindowDetalleProducto ventanaDetalle = new(idProducto)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventanaDetalle.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Reactiva un producto previamente desactivado.
        /// </summary>
        private void BtnReactivarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button? botonPulsado = sender as Button;

                if (botonPulsado?.DataContext is Concepto productoDeLaFila)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar el producto {productoDeLaFila.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (cs.Reactivar(productoDeLaFila.IdConcepto))
                        {
                            MessageBox.Show("Producto reactivado correctamente.",
                                "Información",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo reactivar el producto.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar producto: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}