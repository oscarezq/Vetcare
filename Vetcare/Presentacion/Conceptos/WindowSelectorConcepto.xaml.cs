using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Facturas
{
    public partial class WindowSelectorConcepto : Window
    {
        // Esta es la propiedad que leerá WindowFactura
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
                // Cargamos todos los conceptos (Servicios y Productos)
                _listaOriginal = _conceptoService.ObtenerTodos();
                dgConceptos.ItemsSource = _listaOriginal;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar conceptos: " + ex.Message);
            }
        }

        private void txtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Filtro rápido en memoria
            string busqueda = txtBusqueda.Text.ToLower();
            var filtrados = _listaOriginal.Where(c =>
                c.Nombre.ToLower().Contains(busqueda) ||
                c.Tipo.ToLower().Contains(busqueda)
            ).ToList();

            dgConceptos.ItemsSource = filtrados;
        }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarYSalir();
        }

        private void dgConceptos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SeleccionarYSalir();
        }

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
                MessageBox.Show("Por favor, seleccione un elemento de la lista.");
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}