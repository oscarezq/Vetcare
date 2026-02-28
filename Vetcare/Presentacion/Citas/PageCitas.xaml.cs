using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Página encargada de mostrar, filtrar, ordenar y gestionar las citas de la clínica.
    /// </summary>
    public partial class PageCitas : Page
    {
        // Lista que almacena todas las citas obtenidas de la base de datos.
        private List<Cita> listaCompleta = new List<Cita>();

        // Servicio de la capa de negocio para la gestión de operaciones de citas.
        CitaService cs = new CitaService();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageCitas"/>.
        /// </summary>
        public PageCitas()
        {
            InitializeComponent();

            // Cargar los datos iniciales al abrir la página
            CargarDatos();
        }

        /// <summary>
        /// Recupera la información de las citas desde la Capa de Negocio.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Obtener la colección completa de citas desde el servicio
                listaCompleta = cs.ObtenerTodas();

                // Refresca la interfaz de usuario
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Procesa la lista aplicando filtros de texto, fechas, horas y criterios de ordenación.
        /// </summary>
        private void ActualizarTabla()
        {
            try
            {
                // Validación de seguridad para evitar errores en tiempo de diseño o carga inicial
                if (listaCompleta == null || rbAsc == null || cbOrdenarPor == null || dgCitas == null)
                    return;

                // FILTRADO
                List<Cita> listaFiltrada = new List<Cita>();

                foreach (Cita c in listaCompleta)
                {
                    // 1. Preparación de términos de búsqueda (minúsculas para búsqueda insensible a mayúsculas)
                    string mascotaBusca = txtBuscaPaciente.Text.ToLower();
                    string veteBusca = txtBuscaVeterinario.Text.ToLower();
                    string duenoBusca = txtBuscaDueno.Text.ToLower();

                    string estadoBusca = "";
                    if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado)
                    {
                        estadoBusca = itemEstado.Content.ToString().ToLower();
                    }

                    // 2. Filtros de Texto (Mascota, Veterinario y Estado)
                    if (!string.IsNullOrEmpty(mascotaBusca) && !c.NombreMascota.ToLower().Contains(mascotaBusca)) continue;
                    if (!string.IsNullOrEmpty(veteBusca) && !c.NombreVeterinario.ToLower().Contains(veteBusca)) continue;
                    if (!string.IsNullOrEmpty(duenoBusca) && !c.NombreDueno.ToLower().Contains(duenoBusca)) continue;
                    if (estadoBusca != "" && estadoBusca != "todos")
                    {
                        if (c.Estado == null || !c.Estado.ToLower().Equals(estadoBusca)) continue;
                    }

                    // 3. Filtros de Fecha (Rango Desde - Hasta)
                    if (dtpFechaDesde.SelectedDate.HasValue && c.FechaHora.Date < dtpFechaDesde.SelectedDate.Value.Date) continue;
                    if (dtpFechaHasta.SelectedDate.HasValue && c.FechaHora.Date > dtpFechaHasta.SelectedDate.Value.Date) continue;

                    // 4. Filtros de Hora (Rango HH:mm)
                    if (TimeSpan.TryParse(txtHoraDesde.Text, out TimeSpan hMin) && c.FechaHora.TimeOfDay < hMin) continue;
                    if (TimeSpan.TryParse(txtHoraHasta.Text, out TimeSpan hMax) && c.FechaHora.TimeOfDay > hMax) continue;

                    // Si pasa todos los filtros, se incluye en el resultado
                    listaFiltrada.Add(c);
                }

                // ORDENACIÓN
                if (cbOrdenarPor.SelectedItem is ComboBoxItem itemOrden)
                {
                    string criterio = itemOrden.Content.ToString();
                    bool esAscendente = (bool)rbAsc.IsChecked;

                    switch (criterio)
                    {
                        case "Fecha":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.FechaHora.CompareTo(y.FechaHora) : y.FechaHora.CompareTo(x.FechaHora));
                            break;
                        case "Mascota":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreMascota.CompareTo(y.NombreMascota) : y.NombreMascota.CompareTo(x.NombreMascota));
                            break;
                        case "Veterinario":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreVeterinario.CompareTo(y.NombreVeterinario) : y.NombreVeterinario.CompareTo(x.NombreVeterinario));
                            break;
                        case "Estado":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Estado.CompareTo(y.Estado) : y.Estado.CompareTo(x.Estado));
                            break;
                    }
                }

                // Asignación de la lista procesada al DataGrid
                dgCitas.ItemsSource = listaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar tabla: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- EVENTOS DE INTERFAZ ---

        /// <summary>
        /// Evento genérico que se dispara al cambiar cualquier filtro de texto o selección.
        /// </summary>
        private void FiltroCita_Changed(object sender, EventArgs e)
        {
            ActualizarTabla();
        }

        /// <summary>
        /// Restablece todos los campos de búsqueda a sus valores por defecto.
        /// </summary>
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaPaciente.Clear();
            txtBuscaVeterinario.Clear();
            txtBuscaDueno.Clear();
            cbBuscaEstado.SelectedIndex = 0;
            dtpFechaDesde.SelectedDate = null;
            dtpFechaHasta.SelectedDate = null;
            txtHoraDesde.Clear();
            txtHoraHasta.Clear();
            cbOrdenarPor.SelectedIndex = 0;
            rbDesc.IsChecked = true; // Por defecto solemos querer ver las citas más recientes arriba

            ActualizarTabla();
        }

        // --- ACCIONES CRUD ---

        /// <summary>
        /// Evento para abrir la ventana de nueva cita.
        /// </summary>
        private void btnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            // 1. Instanciamos la ventana
            WindowCita ventanaCita = new WindowCita();

            // 2. La abrimos de forma modal (ShowDialog)
            // ShowDialog detiene la ejecución de esta página hasta que se cierra la ventana
            if (ventanaCita.ShowDialog() == true || true)
            {
                // 3. Al regresar, refrescamos el DataGrid para ver la nueva cita
                CargarDatos();
            }
        }

        /// <summary>
        /// Elimina la cita seleccionada tras confirmar con el usuario.
        /// </summary>
        private void btnEliminarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita cita)
            {
                MessageBoxResult result = MessageBox.Show($"¿Desea cancelar/eliminar la cita de {cita.NombreMascota}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (cs.Eliminar(cita.IdCita))
                    {
                        CargarDatos();
                    }
                }
            }
        }

        /// <summary>
        /// Abre la ventana de edición para la cita seleccionada.
        /// </summary>
        private void btnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita cita)
            {
                // Lógica de edición
                // WindowCita win = new WindowCita(cita);
                // win.ShowDialog();
                // CargarDatos();
            }
        }
    }
}