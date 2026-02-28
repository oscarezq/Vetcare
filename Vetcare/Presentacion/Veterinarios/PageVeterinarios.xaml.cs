using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Veterinarios
{
    /// <summary>
    /// Página encargada de mostrar, filtrar, ordenar y gestionar los veterinarios registrados en el sistema.
    /// </summary>
    public partial class PageVeterinarios : Page
    {
        // Lista completa de veterinarios obtenida de la base de datos
        private List<Veterinario> listaCompleta = new List<Veterinario>();

        // Servicio de negocio para operaciones de veterinarios
        private VeterinarioService vs = new VeterinarioService();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageVeterinarios"/>.
        /// </summary>
        public PageVeterinarios()
        {
            InitializeComponent();

            // Carga inicial de datos en la tabla
            CargarDatos();
        }

        /// <summary>
        /// Recupera la información de todos los veterinarios desde la capa de negocio.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = vs.ObtenerTodos();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar veterinarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filtra, ordena y actualiza el DataGrid con los veterinarios visibles según criterios del usuario.
        /// </summary>
        private void ActualizarTabla()
        {
            try
            {
                if (listaCompleta == null) return;
                if (rbAsc == null || cbOrdenarPor == null || dgVeterinarios == null) return;

                // --- FILTRADO ---
                List<Veterinario> listaFiltrada = new List<Veterinario>();
                foreach (var v in listaCompleta)
                {
                    string nombreBusca = txtBuscaNombre.Text.ToLower();
                    string apellidosBusca = txtBuscaApellidos.Text.ToLower();
                    string especialidadBusca = txtBuscaEspecialidad.Text.ToLower();
                    string numeroColegiadoBusca = txtBuscaNumeroColegiado.Text.ToLower();

                    if (!string.IsNullOrEmpty(nombreBusca) && !v.Nombre.ToLower().Contains(nombreBusca)) continue;
                    if (!string.IsNullOrEmpty(apellidosBusca) && !v.Apellidos.ToLower().Contains(apellidosBusca)) continue;
                    if (!string.IsNullOrEmpty(especialidadBusca) && !v.Especialidad.ToLower().Contains(especialidadBusca)) continue;
                    if (!string.IsNullOrEmpty(numeroColegiadoBusca) && !v.NumeroColegiado.ToLower().Contains(numeroColegiadoBusca)) continue;

                    listaFiltrada.Add(v);
                }

                // --- ORDENACIÓN ---
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cbOrdenarPor.SelectedItem;
                if (itemSeleccionado != null)
                {
                    string criterio = itemSeleccionado.Content.ToString();
                    bool esAscendente = (bool)rbAsc.IsChecked;

                    switch (criterio)
                    {
                        case "Nombre":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Nombre.CompareTo(y.Nombre) : y.Nombre.CompareTo(x.Nombre));
                            break;
                        case "Apellidos":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Apellidos.CompareTo(y.Apellidos) : y.Apellidos.CompareTo(x.Apellidos));
                            break;
                        case "Especialidad":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Especialidad.CompareTo(y.Especialidad) : y.Especialidad.CompareTo(x.Especialidad));
                            break;
                        case "Nº Colegiado":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NumeroColegiado.CompareTo(y.NumeroColegiado) : y.NumeroColegiado.CompareTo(x.NumeroColegiado));
                            break;
                    }
                }

                dgVeterinarios.ItemsSource = listaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar tabla: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- EVENTOS DE FILTRADO Y ORDENACIÓN ---
        private void FiltroAvanzado_Changed(object sender, TextChangedEventArgs e) => ActualizarTabla();
        private void cbOrdenarPor_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void OrdenDirection_Checked(object sender, RoutedEventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Clear();
            txtBuscaApellidos.Clear();
            txtBuscaEspecialidad.Clear();
            txtBuscaNumeroColegiado.Clear();
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

        // --- ACCIONES CRUD ---

        private void btnNuevoVeterinario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowVeterinario ventana = new WindowVeterinario();
                ventana.ShowDialog();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al añadir veterinario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button boton = sender as Button;
                Veterinario v = boton.DataContext as Veterinario;

                if (v != null)
                {
                    WindowVeterinario ventana = new WindowVeterinario(v);
                    ventana.ShowDialog();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar veterinario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button boton = sender as Button;
                Veterinario v = boton.DataContext as Veterinario;

                if (v != null)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Estás seguro de que deseas eliminar a {v.Nombre} {v.Apellidos}?",
                        "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (vs.BorradoLogico(v.IdVeterinario))
                        {
                            MessageBox.Show("Veterinario eliminado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                            CargarDatos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar veterinario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminarVarios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgVeterinarios.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Debes seleccionar al menos un veterinario.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Eliminar {dgVeterinarios.SelectedItems.Count} veterinario(s)?",
                    "Confirmar eliminación múltiple", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    List<int> idsAEliminar = new List<int>();
                    foreach (Veterinario v in dgVeterinarios.SelectedItems)
                    {
                        idsAEliminar.Add(v.IdVeterinario);
                    }

                    if (vs.BorradoLogicoVarios(idsAEliminar))
                    {
                        MessageBox.Show("Veterinarios eliminados correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarDatos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar veterinarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}