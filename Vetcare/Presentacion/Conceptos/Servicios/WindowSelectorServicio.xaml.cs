using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Lógica de interacción para WindowSelectorServicio.xaml
    /// </summary>
    public partial class WindowSelectorServicio : Window
    {
        // Lista en memoria de todos los servicios cargados
        private List<Concepto>? listaServicios;

        // Servicio seleccionado que se devolverá a la ventana padre
        public Concepto? ServicioSeleccionado;

        // Servicio de acceso a datos (negocio)
        private readonly ConceptoService servicioService = new();

        public WindowSelectorServicio()
        {
            InitializeComponent();

            // Carga inicial de datos
            CargarLista();
        }

        /// <summary>
        /// Carga todos los servicios desde la base de datos
        /// </summary>
        private void CargarLista()
        {
            try
            {
                // Obtener lista de servicios desde la capa de negocio
                listaServicios = servicioService.ObtenerServicios();

                // Asignar al DataGrid
                dgServicios.ItemsSource = listaServicios;
            }
            catch (Exception ex)
            {
                // Mostrar error si falla la carga
                MessageBox.Show(
                    "Error al cargar servicios: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filtra servicios por nombre o descripción mientras se escribe
        /// </summary>
        private void TxtBuscaServicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaServicios == null) return;

            // Texto de búsqueda normalizado
            string? busqueda = txtBuscaServicio.Text.ToLower().Trim();

            // Filtrado en memoria
            var filtrados = listaServicios.Where(s =>
                (s.Nombre != null && s.Nombre.ToLower().Contains(busqueda)) ||
                (s.Descripcion != null && s.Descripcion.ToLower().Contains(busqueda))
            ).ToList();

            // Actualizar DataGrid con resultados
            dgServicios.ItemsSource = filtrados;
        }

        /// <summary>
        /// Finaliza la selección del servicio
        /// </summary>
        private void FinalizarSeleccion()
        {
            if (dgServicios.SelectedItem is Concepto serv)
            {
                // Guardar servicio seleccionado
                ServicioSeleccionado = serv;

                // Cerrar ventana devolviendo OK
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Avisar si no hay selección
                MessageBox.Show(
                    "Por favor, seleccione un servicio de la lista.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Doble click en grid selecciona servicio
        /// </summary>
        private void DgServicios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            => FinalizarSeleccion();

        /// <summary>
        /// Botón seleccionar
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
            => FinalizarSeleccion();

        /// <summary>
        /// Botón cancelar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
            => this.Close();

        /// <summary>
        /// Crear nuevo servicio y recargar lista
        /// </summary>
        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de creación de servicio
            WindowServicio win = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (win.ShowDialog() == true)
            {
                // Recargar lista si se ha creado correctamente
                CargarLista();
            }
        }
    }
}