using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Conceptos.Servicios;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado y operaciones
    /// sobre los servicios del sistema.
    /// Permite listar, filtrar, ordenar, crear, editar, eliminar, reactivar y ver detalles de servicios.
    /// </summary>
    public partial class PageServicios : Page
    {
        // Lista completa de servicios cargados desde la base de datos.
        private List<Concepto> listaCompleta = new();

        // Servicio de negocio para la gestión de servicios (conceptos).
        private readonly ConceptoService cs = new();

        /// <summary>
        /// Constructor de la página de servicios.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageServicios()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los servicios desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerServicios();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar servicios: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aplica los filtros, búsquedas y ordenación sobre la lista de servicios.
        /// </summary>
        private void ActualizarTabla()
        {
            // Verificación para evitar errores durante la inicialización de componentes
            if (listaCompleta == null || rbAsc == null || dgServicios == null)
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por nombre
            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
                filtrado = filtrado.Where(s => s.Nombre!.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

            // Filtro por precio mínimo
            if (decimal.TryParse(txtPrecioMin.Text, out decimal pMin))
                filtrado = filtrado.Where(s => s.Precio >= pMin);

            // Filtro por precio máximo
            if (decimal.TryParse(txtPrecioMax.Text, out decimal pMax))
                filtrado = filtrado.Where(s => s.Precio <= pMax);

            // Filtro por estado
            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
            {
                if (item.Content.ToString() == "Activo")
                    filtrado = filtrado.Where(s => s.Activo);
                else if (item.Content.ToString() == "Inactivo")
                    filtrado = filtrado.Where(s => !s.Activo);
            }

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Nombre";
            bool ascendente = rbAsc.IsChecked == true;

            filtrado = criterio switch
            {
                "Precio" => ascendente ? filtrado.OrderBy(s => s.Precio) : filtrado.OrderByDescending(s => s.Precio),
                "ID" => ascendente ? filtrado.OrderBy(s => s.IdConcepto) : filtrado.OrderByDescending(s => s.IdConcepto),
                _ => ascendente ? filtrado.OrderBy(s => s.Nombre) : filtrado.OrderByDescending(s => s.Nombre),
            };

            dgServicios.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro.
        /// </summary>
        private void FiltroServicio_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados en la vista.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Text = "";
            txtPrecioMin.Text = "";
            txtPrecioMax.Text = "";

            cbOrdenarPor.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;

            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo servicio.
        /// </summary>
        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowServicio ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar un servicio existente.
        /// </summary>
        private void BtnEditarServicio_Click(object sender, RoutedEventArgs e)
        {
            var servicio = (Concepto)((Button)sender).DataContext;

            WindowServicio ventana = new(servicio)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Elimina un servicio tras confirmación del usuario.
        /// </summary>
        private void BtnEliminarServicio_Click(object sender, RoutedEventArgs e)
        {
            var servicio = (Concepto)((Button)sender).DataContext;

            var result = MessageBox.Show(
                $"¿Estás seguro de eliminar el servicio {servicio.Nombre}?",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                cs.Eliminar(servicio.IdConcepto);
                CargarDatos();
            }
        }

        /// <summary>
        /// Abre la ficha del servicio al hacer doble clic en la tabla.
        /// </summary>
        private void DgServicios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgServicios.SelectedItem is Concepto servicioSeleccionado)
            {
                try
                {
                    AbrirVentanaDetalles(servicioSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir el detalle del servicio: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ventana de detalles del servicio seleccionado.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (dgServicios.SelectedItem is Concepto servicioSeleccionado)
            {
                try
                {
                    AbrirVentanaDetalles(servicioSeleccionado.IdConcepto);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al abrir el detalle del servicio: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ventana de ficha de servicio.
        /// </summary>
        private void AbrirVentanaDetalles(int idServicio)
        {
            WindowDetalleServicio ventanaDetalle = new(idServicio)
            {
                Owner = Window.GetWindow(this)
            };

            if (ventanaDetalle.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Reactiva un servicio previamente desactivado.
        /// </summary>
        private void BtnReactivarServicio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button? botonPulsado = sender as Button;

                if (botonPulsado?.DataContext is Concepto servicioDeLaFila)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar el servicio {servicioDeLaFila.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (cs.Reactivar(servicioDeLaFila.IdConcepto))
                        {
                            MessageBox.Show("Servicio reactivado correctamente.",
                                "Información",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo reactivar el servicio.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar servicio: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}