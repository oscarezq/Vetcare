using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

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
            lblTitulo.Text = "NUEVO PRODUCTO";
            txtStock.Text = "0";
        }

        public WindowProducto(Concepto productoExistente)
        {
            InitializeComponent();
            producto = productoExistente;
            esEdicion = true;
            lblTitulo.Text = "EDITAR PRODUCTO";
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = producto.Nombre;
            txtPrecio.Text = producto.Precio.ToString("N2");
            txtStock.Text = producto.Stock?.ToString() ?? "0";
            txtDescripcion.Text = producto.Descripcion;

            // Seleccionar el IVA en el ComboBox
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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                producto.Nombre = txtNombre.Text.Trim();
                producto.Precio = decimal.Parse(txtPrecio.Text.Trim());
                producto.Stock = int.Parse(txtStock.Text.Trim());
                producto.Descripcion = txtDescripcion.Text.Trim();
                producto.Tipo = "Producto";
                producto.Activo = true;

                // Obtener valor del IVA del ComboBox
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
                MessageBox.Show("Error al procesar el producto: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("Precio base no válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtStock.Text, out _))
            {
                MessageBox.Show("El stock debe ser un número entero.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

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
    }
}