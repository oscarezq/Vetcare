using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Conceptos.Servicios
{
    /// <summary>
    /// Lógica de interacción para WindowDetalleServicio.xaml
    /// </summary>
    public partial class WindowDetalleServicio : Window
    {
        // Servicio que permite acceder a datos de conceptos (servicios)
        private readonly ConceptoService conceptoService = new();

        // Objeto que guarda el servicio actualmente cargado
        private Concepto? servicioActual;

        // Constructor que recibe el ID del servicio a mostrar
        public WindowDetalleServicio(int idServicio)
        {
            InitializeComponent();

            // Cargamos los datos del servicio al abrir la ventana
            CargarDetalles(idServicio);
        }

        /// <summary>
        /// Carga los detalles del servicio desde la base de datos
        /// </summary>
        private void CargarDetalles(int id)
        {
            try
            {
                // 1. Obtener el servicio por ID
                servicioActual = conceptoService.ObtenerPorId(id);

                if (servicioActual != null)
                {
                    // 2. Asignar nombre y descripción
                    txtNombre.Text = servicioActual.Nombre;
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(servicioActual.Descripcion)
                                            ? servicioActual.Descripcion
                                            : "Sin descripción disponible.";

                    // 3. Mostrar IVA (solo porcentaje)
                    txtIva.Text = servicioActual.IvaPorcentaje.ToString();

                    // 4. Calcular precio sin IVA (base imponible)
                    decimal factorIva = 1 + (servicioActual.IvaPorcentaje / 100m);
                    decimal precioSinIva = servicioActual.Precio / factorIva;

                    txtPrecio.Text = precioSinIva.ToString("N2");

                    // 5. Mostrar precio final (ya incluye IVA)
                    txtTotal.Text = servicioActual.Precio.ToString("N2") + " €";
                }
                else
                {
                    // Si no se encuentra el servicio, se muestra error y se cierra ventana
                    MessageBox.Show(
                        "No se ha podido encontrar la información del servicio.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Error crítico al cargar datos
                MessageBox.Show(
                    "Error al cargar detalles: " + ex.Message,
                    "Error Crítico",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cierra la ventana de detalle
        /// </summary>
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}