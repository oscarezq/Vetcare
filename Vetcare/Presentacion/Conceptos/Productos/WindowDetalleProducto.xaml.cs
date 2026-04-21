using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Conceptos.Productos
{
    /// <summary>
    /// Lógica de interacción para WindowDetalleProducto.xaml
    /// </summary>
    public partial class WindowDetalleProducto : Window
    {
        // Servicio para acceder a datos del producto
        private readonly ConceptoService conceptoService = new();

        // Producto cargado actualmente en pantalla
        private Concepto? productoActual;

        // ID del producto que se quiere mostrar
        private readonly int _idProducto;

        // Indica si se actualizó el stock al abrir la ventana de ajustar stock
        public bool SeActualizoStock { get; private set; } = false;

        /// <summary>
        /// Constructor: recibe el ID del producto a mostrar
        /// </summary>
        public WindowDetalleProducto(int idProducto)
        {
            InitializeComponent();

            _idProducto = idProducto;

            // Carga inicial de datos
            CargarDetalles(_idProducto);
        }

        /// <summary>
        /// Carga los datos del producto desde la base de datos y los muestra en pantalla
        /// </summary>
        private void CargarDetalles(int id)
        {
            try
            {
                // Obtener producto desde la capa de negocio
                productoActual = conceptoService.ObtenerPorId(id);

                if (productoActual != null)
                {
                    this.DataContext = productoActual;

                    // Rellenar campos básicos
                    txtNombre.Text = productoActual.Nombre;

                    // Stock (si es null se muestra 0)
                    txtStock.Text = productoActual.Stock?.ToString() ?? "0";

                    // IVA en formato entero
                    txtIva.Text = productoActual.IvaPorcentaje.ToString("N0");

                    // Descripción con fallback si está vacía
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(productoActual.Descripcion)
                                            ? productoActual.Descripcion
                                            : "Sin descripción registrada.";

                    // Precio total con IVA
                    txtTotal.Text = productoActual.Precio.ToString("N2") + " €";

                    // Cálculo del precio sin IVA
                    decimal factorIva = 1 + (productoActual.IvaPorcentaje / 100m);
                    decimal precioSinIva = productoActual.Precio / factorIva;

                    txtPrecio.Text = precioSinIva.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                // Error controlado de carga
                MessageBox.Show("Error al cargar detalles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre la ventana para editar el stock del producto
        /// </summary>
        private void BtnEditarStock_Click(object sender, RoutedEventArgs e)
        {
            if (productoActual == null) return;

            // Abrimos la ventana de ajuste de stock pasando el ID del producto actual
            var ventanaStock = new WindowAjustarStock(productoActual.IdConcepto);

            // Si el usuario confirma cambios
            if (ventanaStock.ShowDialog() == true)
            {
                SeActualizoStock = true;

                // Recargar datos después del cambio de stock
                CargarDetalles(_idProducto);

                MessageBox.Show(
                    "El stock se ha actualizado correctamente en la base de datos.",
                    "Inventario Actualizado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        /// <summary>
        /// Cierra la ventana de detalle
        /// </summary>
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = SeActualizoStock;
            this.Close();
        }
    }
}