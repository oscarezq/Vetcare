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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Conceptos.Servicios;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Lógica de interacción para PageServicios.xaml
    /// </summary>
    public partial class PageServicios : Page
    {
        private ConceptoService conceptoService = new ConceptoService();
        private List<Concepto> listaCompleta = new List<Concepto>();

        public PageServicios()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                listaCompleta = conceptoService.ObtenerServicios();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AplicarFiltros()
        {
            if (listaCompleta == null || rbAsc == null || dgServicios == null) return;

            IEnumerable<Concepto> filtrados = listaCompleta;

            // 1. Filtro por Nombre
            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
            {
                filtrados = filtrados.Where(s => s.Nombre.ToLower().Contains(txtBuscaNombre.Text.ToLower()));
            }

            // 2. Filtro por Precio Mínimo
            if (decimal.TryParse(txtPrecioMin.Text, out decimal pMin))
            {
                filtrados = filtrados.Where(s => s.Precio >= pMin);
            }

            // 3. Filtro por Precio Máximo
            if (decimal.TryParse(txtPrecioMax.Text, out decimal pMax))
            {
                filtrados = filtrados.Where(s => s.Precio <= pMax);
            }

            // 4. Ordenación
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool ascendente = rbAsc.IsChecked == true;

            switch (criterio)
            {
                case "Precio":
                    filtrados = ascendente ? filtrados.OrderBy(s => s.Precio) : filtrados.OrderByDescending(s => s.Precio);
                    break;
                case "ID":
                    filtrados = ascendente ? filtrados.OrderBy(s => s.IdConcepto) : filtrados.OrderByDescending(s => s.IdConcepto);
                    break;
                default: // Nombre
                    filtrados = ascendente ? filtrados.OrderBy(s => s.Nombre) : filtrados.OrderByDescending(s => s.Nombre);
                    break;
            }

            dgServicios.ItemsSource = filtrados.ToList();
        }

        private void FiltroServicio_Changed(object sender, TextChangedEventArgs e) => AplicarFiltros();

        private void FiltroServicio_Changed(object sender, SelectionChangedEventArgs e) => AplicarFiltros();

        private void FiltroServicio_Changed(object sender, RoutedEventArgs e) => AplicarFiltros();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Text = "";
            txtPrecioMin.Text = "";
            txtPrecioMax.Text = "";
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            AplicarFiltros();
        }

        private void btnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            // Aquí llamarías a tu WindowServicio (Formulario)
            WindowServicio win = new WindowServicio();
            if (win.ShowDialog() == true) 
                CargarDatos();
        }

        private void btnEditarServicio_Click(object sender, RoutedEventArgs e)
        {
            var servicio = (Concepto)((Button)sender).DataContext;
            WindowServicio win = new WindowServicio(servicio);
            if (win.ShowDialog() == true) CargarDatos();
        }

        private void btnEliminarServicio_Click(object sender, RoutedEventArgs e)
        {
            var servicio = (Concepto)((Button)sender).DataContext;
            var result = MessageBox.Show($"¿Estás seguro de eliminar el servicio {servicio.Nombre}?",
                                       "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                
                CargarDatos();
            }
        }

        private void dgServicios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgServicios.SelectedItem is Concepto conceptoSeleccionado)
            {
                try
                {
                    abrirVentanaDetalles(conceptoSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir el detalle: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is Concepto conceptoSeleccionado)
            {
                try
                {
                    abrirVentanaDetalles(conceptoSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir el detalle: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void abrirVentanaDetalles(int idServicio)
        {
            WindowDetalleServicio ventanaDetalle = new WindowDetalleServicio(idServicio);

            ventanaDetalle.ShowDialog();
        }
    }
}
