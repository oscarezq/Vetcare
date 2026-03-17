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

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Lógica de interacción para WindowSelectorServicio.xaml
    /// </summary>
    public partial class WindowSelectorServicio : Window
    {
        private List<Concepto> listaServicios;
        public Concepto ServicioSeleccionado { get; set; }
        private ConceptoService servicioService = new ConceptoService();

        public WindowSelectorServicio()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            try
            {
                listaServicios = servicioService.ObtenerServicios();
                dgServicios.ItemsSource = listaServicios;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar servicios: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtBuscaServicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaServicios == null) return;

            string busqueda = txtBuscaServicio.Text.ToLower().Trim();

            // Filtrado por Nombre y Descripción
            var filtrados = listaServicios.Where(s =>
                (s.Nombre != null && s.Nombre.ToLower().Contains(busqueda)) ||
                (s.Descripcion != null && s.Descripcion.ToLower().Contains(busqueda))
            ).ToList();

            dgServicios.ItemsSource = filtrados;
        }

        private void FinalizarSeleccion()
        {
            if (dgServicios.SelectedItem is Concepto serv)
            {
                ServicioSeleccionado = serv;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un servicio de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgServicios_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            // Ajusta el nombre de tu ventana de edición de servicios si es diferente
            WindowServicio win = new WindowServicio();
            if (win.ShowDialog() == true)
            {
                CargarLista();
            }
        }
    }
}
