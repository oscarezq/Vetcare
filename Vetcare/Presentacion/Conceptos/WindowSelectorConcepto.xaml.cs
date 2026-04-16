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
    /// <summary>
    /// Ventana de selección de conceptos (productos y servicios)
    /// utilizada para añadir líneas a una factura.
    /// </summary>
    public partial class WindowSelectorConcepto : Window
    {
        // Concepto seleccionado por el usuario (producto o servicio).
        public Concepto? ConceptoSeleccionado;

        // Servicio de acceso a datos de conceptos.
        private readonly ConceptoService _conceptoService = new();

        // Lista original completa sin filtrar.
        private List<Concepto>? _listaOriginal = new();

        /// <summary>
        /// Constructor de la ventana.
        /// </summary>
        public WindowSelectorConcepto()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los conceptos desde la base de datos
        /// y los muestra en el DataGrid.
        /// </summary>
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

        // Evento único para manejar filtros (texto + tipo de concepto)
        private void Filtro_Changed(object sender, EventArgs e)
        {
            if (_listaOriginal == null || dgConceptos == null) return;

            string busqueda = txtBusqueda.Text.ToLower();

            // Tipo seleccionado en el ComboBox (Todos / Producto / Servicio)
            string tipoSeleccionado = (cbTipoFiltro.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Todos";

            var filtrados = _listaOriginal.Where(c =>
            {
                // Coincidencia por nombre
                bool coincideNombre = c.Nombre!.ToLower().Contains(busqueda);

                // Coincidencia por tipo (o todos)
                bool coincideTipo = tipoSeleccionado == "Todos" ||
                                     c.Tipo!.Equals(tipoSeleccionado, StringComparison.OrdinalIgnoreCase);

                return coincideNombre && coincideTipo;
            }).ToList();

            dgConceptos.ItemsSource = filtrados;
        }

        /// <summary>
        /// Botón seleccionar concepto.
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
            => SeleccionarYSalir();

        /// <summary>
        /// Selección con doble click en el DataGrid.
        /// </summary>
        private void DgConceptos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => SeleccionarYSalir();

        /// <summary>
        /// Valida la selección y cierra la ventana devolviendo el resultado.
        /// </summary>
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
                MessageBox.Show("Por favor, seleccione un elemento de la lista.",
                                "Aviso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Cierra la ventana sin selección.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Abre la ventana de creación de producto.
        /// </summary>
        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            WindowProducto win = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (win.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana de creación de servicio.
        /// </summary>
        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowServicio win = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (win.ShowDialog() == true)
                CargarDatos();
        }
    }
}