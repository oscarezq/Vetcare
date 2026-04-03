using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Conceptos; // Asegúrate de tener la referencia a la ventana de ajuste

namespace Vetcare.Presentacion.Servicios
{
    public partial class WindowProducto : Window
    {
        private Concepto producto;
        private ConceptoService productoService = new ConceptoService();
        private bool esEdicion = false;

        public WindowProducto()
        {
            InitializeComponent();
            producto = new Concepto();
            esEdicion = false;

            // Configuración para Nuevo
            lblTitulo.Text = "NUEVO PRODUCTO";
            txtStock.IsReadOnly = false;
            txtStock.Focusable = true;
            btnAjustarStock.Visibility = Visibility.Collapsed;
            borderStock.Background = System.Windows.Media.Brushes.White;
        }

        public WindowProducto(Concepto productoExistente)
        {
            InitializeComponent();
            producto = productoExistente;
            esEdicion = true;

            // Configuración para Edición
            lblTitulo.Text = "EDITAR PRODUCTO";
            txtStock.IsReadOnly = true;
            txtStock.Focusable = false;
            btnAjustarStock.Visibility = Visibility.Visible;
            borderStock.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#F1F5F9");
            lblInfoStock.Text = "* El stock se gestiona mediante ajustes de inventario.";

            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = producto.Nombre;
            txtPrecio.Text = producto.Precio.ToString("N2");
            txtStock.Text = producto.Stock?.ToString() ?? "0";
            txtDescripcion.Text = producto.Descripcion;

            string ivaGuardado = producto.IvaPorcentaje.ToString("G29");
            foreach (ComboBoxItem item in cmbIva.Items)
            {
                if (item.Content.ToString() == ivaGuardado)
                {
                    cmbIva.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnAjustarStock_Click(object sender, RoutedEventArgs e)
        {
            // Solo entra aquí en modo edición
            var ventanaStock = new WindowAjustarStock(producto.IdConcepto);
            if (ventanaStock.ShowDialog() == true)
            {
                // Refrescamos el stock desde la base de datos
                var actualizado = productoService.ObtenerPorId(producto.IdConcepto);
                if (actualizado != null)
                {
                    producto.Stock = actualizado.Stock;
                    txtStock.Text = producto.Stock.ToString();
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                producto.Nombre = txtNombre.Text.Trim();
                producto.Precio = decimal.Parse(txtPrecio.Text.Trim());
                producto.Descripcion = txtDescripcion.Text.Trim();
                producto.Tipo = "Producto";
                producto.Activo = true;

                // Solo guardamos el stock del TextBox si es un producto nuevo
                if (!esEdicion)
                {
                    producto.Stock = int.Parse(txtStock.Text.Trim());
                }

                if (cmbIva.SelectedItem is ComboBoxItem selectedItem)
                {
                    producto.IvaPorcentaje = decimal.Parse(selectedItem.Content.ToString());
                }

                bool exito = esEdicion ?
                    productoService.Actualizar(producto) :
                    productoService.Insertar(producto);

                if (exito)
                {
                    MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return MostrarError("El nombre es obligatorio.");
            if (!decimal.TryParse(txtPrecio.Text, out _)) return MostrarError("Precio no válido.");
            if (!int.TryParse(txtStock.Text, out _)) return MostrarError("El stock debe ser un número entero.");
            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumber = System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9,]");
            if (!isNumber || (e.Text == "," && ((TextBox)sender).Text.Contains(",")))
                e.Handled = true;
        }

        private void txtSoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]");
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}