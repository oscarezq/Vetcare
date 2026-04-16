using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Ventana para seleccionar un producto desde un listado filtrable
    /// </summary>
    public partial class WindowSelectorProducto : Window
    {
        // Lista completa de productos cargados desde la base de datos
        private List<Concepto>? listaProductos;

        // Producto que el usuario selecciona en la ventana
        public Concepto? ProductoSeleccionado;

        // Servicio de acceso a datos de conceptos/productos
        private readonly ConceptoService conceptoService = new();

        /// <summary>
        /// Inicializa la ventana y carga los productos
        /// </summary>
        public WindowSelectorProducto()
        {
            InitializeComponent();

            // Carga inicial del listado
            CargarLista();
        }

        /// <summary>
        /// Obtiene productos desde la base de datos y los muestra en el DataGrid
        /// </summary>
        private void CargarLista()
        {
            try
            {
                // Obtener productos desde la capa de negocio
                listaProductos = conceptoService.ObtenerProductos();

                // Asignar al DataGrid
                dgProductos.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                // Error controlado de carga
                MessageBox.Show(
                    "Error al cargar productos: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filtra productos según texto introducido en el buscador
        /// </summary>
        private void TxtBuscaProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Si no hay datos aún, no hace nada
            if (listaProductos == null) return;

            // Texto de búsqueda normalizado
            string busqueda = txtBuscaProducto.Text.ToLower().Trim();

            // Filtrado por nombre o descripción
            var filtrados = listaProductos.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(busqueda)) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(busqueda))
            ).ToList();

            // Actualizar DataGrid
            dgProductos.ItemsSource = filtrados;
        }

        /// <summary>
        /// Abrir ventana de creación de producto
        /// </summary>
        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de producto en modo creación
            WindowProducto win = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Si se creó correctamente, recargar lista
            if (win.ShowDialog() == true)
                CargarLista();
        }

        /// <summary>
        /// Finaliza la selección del producto activo del DataGrid
        /// </summary>
        private void FinalizarSeleccion()
        {
            if (dgProductos.SelectedItem is Concepto prod)
            {
                // Validación de stock
                if (prod.Stock <= 0)
                {
                    var result = MessageBox.Show(
                        "El producto seleccionado no tiene stock. ¿Desea continuar?",
                        "Aviso",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                        return;
                }

                // Asignar resultado
                ProductoSeleccionado = prod;

                // Confirmar selección y cerrar
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Ningún producto seleccionado
                MessageBox.Show(
                    "Por favor, seleccione un producto de la lista.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Doble click en el DataGrid = seleccionar producto
        /// </summary>
        private void DgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();

        /// <summary>
        /// Botón seleccionar producto
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();

        /// <summary>
        /// Cerrar ventana sin selección
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}