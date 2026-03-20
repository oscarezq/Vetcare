using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Servicios;

namespace Vetcare.Presentacion.Facturas
{
    public partial class WindowSelectorConcepto : Window
    {
        public Concepto ConceptoSeleccionado { get; private set; }
        private ConceptoService _conceptoService = new ConceptoService();
        private List<Concepto> _listaOriginal = new List<Concepto>();

        public WindowSelectorConcepto()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                _listaOriginal = _conceptoService.ObtenerTodos();
                dgConceptos.ItemsSource = _listaOriginal;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
        }

        // Evento único para manejar ambos filtros (Texto y ComboBox)
        private void Filtro_Changed(object sender, EventArgs e)
        {
            if (_listaOriginal == null || dgConceptos == null) return;

            string busqueda = txtBusqueda.Text.ToLower();
            string tipoSeleccionado = (cbTipoFiltro.SelectedItem as ComboBoxItem)?.Content.ToString();

            var filtrados = _listaOriginal.Where(c => {
                bool coincideNombre = c.Nombre.ToLower().Contains(busqueda);
                bool coincideTipo = tipoSeleccionado == "Todos" || c.Tipo.Equals(tipoSeleccionado, StringComparison.OrdinalIgnoreCase);
                return coincideNombre && coincideTipo;
            }).ToList();

            dgConceptos.ItemsSource = filtrados;
        }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => SeleccionarYSalir();

        private void dgConceptos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => SeleccionarYSalir();

        private void SeleccionarYSalir()
        {
            if (dgConceptos.SelectedItem is Concepto seleccionado)
            {
                ConceptoSeleccionado = seleccionado;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un elemento de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Métodos para los botones de añadir (deberás llamar a tus ventanas de creación)
        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            WindowProducto win = new WindowProducto();
            if(win.ShowDialog() == true) CargarDatos();
        }

        private void btnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowServicio win = new WindowServicio();
            if(win.ShowDialog() == true) CargarDatos();
        }
    }
}