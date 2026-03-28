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
                    // Asignar textos a los TextBlocks del XAML
                    txtNombre.Text = servicioActual.Nombre;
                    txtPrecio.Text = servicioActual.Precio.ToString("N2"); // Formato 0.00
                    txtIva.Text = servicioActual.IvaPorcentaje.ToString();
                    txtDescripcion.Text = !string.IsNullOrWhiteSpace(servicioActual.Descripcion)
                                          ? servicioActual.Descripcion
                                          : "Sin descripción disponible.";

                    // Calcular y mostrar el PVP Total
                    decimal precioFinal = servicioActual.Precio * (1 + (servicioActual.IvaPorcentaje / 100m));
                    txtTotal.Text = precioFinal.ToString("N2") + " €";
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
