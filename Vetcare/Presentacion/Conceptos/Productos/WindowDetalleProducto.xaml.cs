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
                    txtIva.Text = productoActual.IvaPorcentaje.ToString("N0");
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(productoActual.Descripcion)
                                            ? productoActual.Descripcion
                                            : "Sin descripción registrada.";

                    // --- Lógica de Precios Invertida ---

                    // 1. El total es directamente el precio que viene de la BD (ya tiene IVA)
                    txtTotal.Text = productoActual.Precio.ToString("N2") + " €";

                    // 2. El precio mostrado en 'txtPrecio' será la Base Imponible (Precio sin IVA)
                    decimal factorIva = 1 + (productoActual.IvaPorcentaje / 100m);
                    decimal precioSinIva = productoActual.Precio / factorIva;
                    txtPrecio.Text = precioSinIva.ToString("N2");

                    // --- Lógica Visual de Stock ---
                    if (productoActual.Stock <= 5)
                    {
                        txtStock.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else
                    {
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