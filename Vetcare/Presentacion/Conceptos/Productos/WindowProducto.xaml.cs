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

        public WindowProducto() // Constructor para Nuevo
        {
            InitializeComponent();
            producto = new Concepto();
            esEdicion = false;
            lblTitulo.Text = "NUEVO PRODUCTO";
            txtStock.Text = "0";
        }

        public WindowProducto(Concepto productoExistente) // Constructor para Editar
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
            txtPrecio.Text = producto.PrecioBase.ToString("N2");
            txtIva.Text = producto.IvaPorcentaje.ToString("N2");
            txtStock.Text = producto.Stock?.ToString() ?? "0";
            txtDescripcion.Text = producto.Descripcion;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                producto.Nombre = txtNombre.Text.Trim();
                producto.PrecioBase = decimal.Parse(txtPrecio.Text.Trim());
                producto.IvaPorcentaje = decimal.Parse(txtIva.Text.Trim());
                producto.Stock = int.Parse(txtStock.Text.Trim());
                producto.Descripcion = txtDescripcion.Text.Trim();
                producto.Tipo = "Producto";
                producto.Activo = true;

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
                MessageBox.Show("Error al procesar el producto: " + ex.Message);
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

        // Validar solo números y coma para precio
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumber = System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9,]");
            if (!isNumber || (e.Text == "," && ((TextBox)sender).Text.Contains(",")))
                e.Handled = true;
        }

        // Validar SOLO números enteros para Stock
        private void txtSoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]");
        }

        private void txtIva_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor))
            {
                if (valor > 100) txtIva.Text = "100";
                else if (valor < 0) txtIva.Text = "0";
            }
            else txtIva.Text = "21";
        }

        private void btnSubirIva_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor) && valor < 100)
                txtIva.Text = (valor + 1).ToString();
        }

        private void btnBajarIva_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor) && valor > 0)
                txtIva.Text = (valor - 1).ToString();
        }
    }
}