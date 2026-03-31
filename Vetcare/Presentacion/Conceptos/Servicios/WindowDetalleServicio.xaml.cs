using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Conceptos.Servicios
{
    /// <summary>
    /// Lógica de interacción para WindowDetalleServicio.xaml
    /// </summary>
    public partial class WindowDetalleServicio : Window
    {
        private ConceptoService conceptoService = new ConceptoService();
        private Concepto servicioActual;

        // Constructor que recibe el ID del servicio
        public WindowDetalleServicio(int idServicio)
        {
            InitializeComponent();
            CargarDetalles(idServicio);
        }

        private void CargarDetalles(int id)
        {
            try
            {
                servicioActual = conceptoService.ObtenerPorId(id);

                if (servicioActual != null)
                {
                    // 1. Asignar nombre y descripción
                    txtNombre.Text = servicioActual.Nombre;
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(servicioActual.Descripcion)
                                            ? servicioActual.Descripcion
                                            : "Sin descripción disponible.";

                    // 2. Mostrar el IVA (solo el porcentaje)
                    txtIva.Text = servicioActual.IvaPorcentaje.ToString();

                    // 3. CALCULAR PRECIO SIN IVA (Base Imponible)
                    // Como servicioActual.Precio ya trae el IVA, dividimos por (1 + IVA/100)
                    decimal factorIva = 1 + (servicioActual.IvaPorcentaje / 100m);
                    decimal precioSinIva = servicioActual.Precio / factorIva;

                    txtPrecio.Text = precioSinIva.ToString("N2"); // Ahora muestra el precio base

                    // 4. MOSTRAR TOTAL (Ya tiene el IVA incluido)
                    txtTotal.Text = servicioActual.Precio.ToString("N2") + " €";
                }
                else
                {
                    MessageBox.Show("No se ha podido encontrar la información del servicio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalles: " + ex.Message, "Error Crítico", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
