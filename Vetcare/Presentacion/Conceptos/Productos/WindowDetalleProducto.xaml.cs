using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Conceptos.Productos
{
    public partial class WindowDetalleProducto : Window
    {
        private ConceptoService conceptoService = new ConceptoService();
        private Concepto productoActual;
        private int _idProducto; // Guardamos el ID para refrescar después

        public WindowDetalleProducto(int idProducto)
        {
            InitializeComponent();
            _idProducto = idProducto;
            CargarDetalles(_idProducto);
        }

        private void CargarDetalles(int id)
        {
            try
            {
                productoActual = conceptoService.ObtenerPorId(id);

                if (productoActual != null)
                {
                    txtNombre.Text = productoActual.Nombre;
                    txtStock.Text = productoActual.Stock?.ToString() ?? "0";
                    txtPrecio.Text = productoActual.Precio.ToString("N2");
                    txtIva.Text = productoActual.IvaPorcentaje.ToString("N0");
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(productoActual.Descripcion)
                                          ? productoActual.Descripcion
                                          : "Sin descripción registrada.";

                    // Cálculo PVP
                    decimal precioFinal = productoActual.Precio * (1 + (productoActual.IvaPorcentaje / 100m));
                    txtTotal.Text = precioFinal.ToString("N2") + " €";

                    // Cambiar color del stock si es bajo (ej. 5 o menos)
                    if (productoActual.Stock <= 5)
                    {
                        txtStock.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else
                    {
                        // Importante resetear el color si el stock sube
                        txtStock.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#15803D");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditarStock_Click(object sender, RoutedEventArgs e)
        {
            if (productoActual == null) return;

            // 1. Abrimos la ventana de ajuste pasando el ID del producto actual
            var ventanaStock = new WindowAjustarStock(productoActual.IdConcepto);

            // 2. Si se guardaron los cambios (DialogResult = true)
            if (ventanaStock.ShowDialog() == true)
            {
                // 3. Refrescamos los datos volviendo a llamar a CargarDetalles
                CargarDetalles(_idProducto);

                MessageBox.Show(
                    "El stock se ha actualizado correctamente en la base de datos.", // Mensaje
                    "Inventario Actualizado",                                         // Título de la ventana
                    MessageBoxButton.OK,                                             // Botón de Aceptar
                    MessageBoxImage.Information                                      // Icono azul de información
                );

                // Opcional: Avisar al usuario que la ventana principal también debe refrescarse
                this.DialogResult = true;
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}