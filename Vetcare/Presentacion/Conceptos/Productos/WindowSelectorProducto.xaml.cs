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
    public partial class WindowSelectorProducto : Window
    {
        private List<Concepto> listaProductos;
        public Concepto ProductoSeleccionado { get; set; }
        private ConceptoService conceptoService = new ConceptoService();

        public WindowSelectorProducto()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            try
            {
                // Filtramos por tipo "Producto" desde el servicio o por LINQ
                listaProductos = conceptoService.ObtenerProductos();
                dgProductos.ItemsSource = listaProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtBuscaProducto_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaProductos == null) return;

            string busqueda = txtBuscaProducto.Text.ToLower().Trim();

            var filtrados = listaProductos.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(busqueda)) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(busqueda))
            ).ToList();

            dgProductos.ItemsSource = filtrados;
        }

        private void FinalizarSeleccion()
        {
            if (dgProductos.SelectedItem is Concepto prod)
            {
                // Validación opcional: ¿Deseas permitir seleccionar productos sin stock?
                if (prod.Stock <= 0)
                {
                    var result = MessageBox.Show("El producto seleccionado no tiene stock. ¿Desea continuar?", "Aviso", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) return;
                }

                ProductoSeleccionado = prod;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            WindowProducto win = new WindowProducto();
            if (win.ShowDialog() == true)
            {
                CargarLista();
            }
        }
    }
}