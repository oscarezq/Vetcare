using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Conceptos;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Ventana para crear o editar un producto dentro del sistema.
    /// Permite gestionar datos básicos, IVA y stock.
    /// </summary>
    public partial class WindowProducto : Window
    {
        // Objeto producto que se está creando o editando
        private readonly Concepto producto;

        // Servicio de acceso a datos de productos
        private readonly ConceptoService productoService = new();

        // Indica si estamos en modo edición o creación
        private readonly bool esEdicion = false;

        [GeneratedRegex("[0-9,]")]
        private static partial Regex RegexDecimal();

        [GeneratedRegex("[0-9]")]
        private static partial Regex RegexEnteros();

        /// <summary>
        /// Constructor para crear un nuevo producto.
        /// </summary>
        public WindowProducto()
        {
            InitializeComponent();
            producto = new Concepto();
            esEdicion = false;

            // Configuración visual para nuevo producto
            lblTitulo.Text = "NUEVO PRODUCTO";
            txtStock.IsReadOnly = false;
            txtStock.Focusable = true;
            btnAjustarStock.Visibility = Visibility.Collapsed;
            borderStock.Background = Brushes.White;
        }

        /// <summary>
        /// Constructor para editar un producto existente.
        /// </summary>
        /// <param name="productoExistente">Producto a editar</param>
        public WindowProducto(Concepto productoExistente)
        {
            InitializeComponent();
            producto = productoExistente;
            esEdicion = true;

            // Configuración visual para edición
            lblTitulo.Text = "EDITAR PRODUCTO";
            txtStock.IsReadOnly = true;
            txtStock.Focusable = false;
            btnAjustarStock.Visibility = Visibility.Visible;
            borderStock.Background = new SolidColorBrush(Color.FromRgb(241, 245, 249));
            lblInfoStock.Text = "* El stock se gestiona mediante ajustes de inventario.";

            CargarDatos();
        }

        /// <summary>
        /// Carga los datos del producto en los controles de la UI.
        /// </summary>
        private void CargarDatos()
        {
            txtNombre.Text = producto.Nombre;
            txtPrecio.Text = producto.Precio.ToString("N2");
            txtStock.Text = producto.Stock?.ToString() ?? "0";
            txtDescripcion.Text = producto.Descripcion;

            // Selecciona el IVA correspondiente en el ComboBox
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

        /// <summary>
        /// Abre la ventana para ajustar el stock del producto.
        /// </summary>
        private void BtnAjustarStock_Click(object sender, RoutedEventArgs e)
        {
            // Solo disponible en modo edición
            var ventanaStock = new WindowAjustarStock(producto.IdConcepto);

            if (ventanaStock.ShowDialog() == true)
            {
                // Refrescar el stock desde la base de datos
                var actualizado = productoService.ObtenerPorId(producto.IdConcepto);
                if (actualizado != null)
                {
                    producto.Stock = actualizado.Stock;
                    txtStock.Text = producto.Stock.ToString();
                }
            }
        }

        /// <summary>
        /// Guarda el producto en la base de datos (crear o actualizar).
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validación previa
            if (!Validar()) return;

            try
            {
                // Asignación de valores desde la UI
                producto.Nombre = txtNombre.Text.Trim();
                producto.Precio = decimal.Parse(txtPrecio.Text.Trim());
                producto.Descripcion = txtDescripcion.Text.Trim();
                producto.Tipo = "Producto";
                producto.Activo = true;

                // Solo asignar stock manual en creación
                if (!esEdicion)
                    producto.Stock = int.Parse(txtStock.Text.Trim());

                // Obtener IVA seleccionado
                if (cmbIva.SelectedItem is ComboBoxItem selectedItem)
                    producto.IvaPorcentaje = decimal.Parse(selectedItem.Content.ToString() ?? "");

                // Insertar o actualizar según el modo
                bool exito = esEdicion
                    ? productoService.Actualizar(producto)
                    : productoService.Insertar(producto);

                if (exito)
                {
                    MessageBox.Show("Producto guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida los datos introducidos en el formulario.
        /// </summary>
        /// <returns>True si todo es correcto, False si hay errores</returns>
        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre es obligatorio.");

            if (!decimal.TryParse(txtPrecio.Text, out _))
                return MostrarError("Precio no válido.");

            if (!int.TryParse(txtStock.Text, out _))
                return MostrarError("El stock debe ser un número entero.");

            return true;
        }

        /// <summary>
        /// Muestra un mensaje de error de validación.
        /// </summary>
        private static bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        /// <summary>
        /// Permite solo números y coma en el campo precio.
        /// </summary>
        private void TxtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            bool isNumber = RegexDecimal().IsMatch(e.Text);

            if (!isNumber || (e.Text == "," && tb.Text.Contains(',')))
                e.Handled = true;
        }

        /// <summary>
        /// Permite solo números enteros en el campo stock.
        /// </summary>
        private void TxtSoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !RegexEnteros().IsMatch(e.Text);
        }

        /// <summary>
        /// Cierra la ventana sin guardar cambios.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}