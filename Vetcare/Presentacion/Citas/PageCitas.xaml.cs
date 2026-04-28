using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Página de gestión y listado de citas
    /// Permite filtrar, ordenar, crear, editar y ver detalles de citas
    /// </summary>
    public partial class PageCitas : Page
    {
        // Lista completa de citas cargadas desde la base de datos
        private List<Cita> listaCompleta = new();

        // Servicio de lógica de negocio para citas
        readonly CitaService cs = new();

        /// <summary>
        /// Constructor de la página
        /// </summary>
        public PageCitas()
        {
            InitializeComponent();

            // Control de visibilidad según rol del usuario
            if (Sesion.UsuarioActual!.IdRol == 1 || Sesion.UsuarioActual.IdRol == 3)
            {
                // Oculta selector de vista si es admin o recepcionista
                brdSelectorVista.Visibility = Visibility.Collapsed;

                // Admin siempre ve todas las citas
                rbTodasCitas.IsChecked = true;
                rbTodasCitas.IsChecked = true;
            }
            else
            {
                // Veterinario empieza viendo solo sus citas
                rbMisCitas.IsChecked = true;
                rbMisCitas.IsChecked = true;
            }

            // Carga inicial de datos
            CargarDatos();
        }

        /// <summary>
        /// Carga todas las citas desde la base de datos
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Obtiene todas las citas
                listaCompleta = cs.ObtenerTodas();

                // Refresca la tabla con filtros aplicados
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aplica filtros, ordenación y actualiza el DataGrid
        /// </summary>
        private void ActualizarTabla()
        {
            // Seguridad: evita errores si aún no está cargado
            if (listaCompleta == null || dgCitas == null) return;

            // Base de datos en memoria para consultas LINQ
            var query = listaCompleta.AsEnumerable();

            // FILTRO: solo mis citas si no es admin y está seleccionado
            if (Sesion.UsuarioActual!.IdRol != 1 && rbMisCitas.IsChecked == true)
            {
                query = query.Where(c => c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario);
            }

            // FILTRO: paciente (mascota)
            if (!string.IsNullOrEmpty(txtBuscaPaciente.Text))
                query = query.Where(c => c.NombreMascota!.ToLower().Contains(txtBuscaPaciente.Text.ToLower()));

            // FILTRO: veterinario
            if (!string.IsNullOrEmpty(txtBuscaVeterinario.Text))
                query = query.Where(c => c.NombreVeterinario!.ToLower().Contains(txtBuscaVeterinario.Text.ToLower()));

            // FILTRO: dueño
            if (!string.IsNullOrEmpty(txtBuscaDueno.Text))
                query = query.Where(c => c.NombreDueno!.ToLower().Contains(txtBuscaDueno.Text.ToLower()));

            // FILTRO: estado
            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
                query = query.Where(c => c.Estado == item.Content.ToString());

            // FILTRO: fecha desde
            if (dtpFechaDesde.SelectedDate.HasValue)
                query = query.Where(c => c.FechaHora.Date >= dtpFechaDesde.SelectedDate.Value.Date);

            // FILTRO: fecha hasta
            if (dtpFechaHasta.SelectedDate.HasValue)
                query = query.Where(c => c.FechaHora.Date <= dtpFechaHasta.SelectedDate.Value.Date);

            // ORDENACIÓN: criterio seleccionado
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Fecha";
            bool asc = rbAsc.IsChecked == true;

            query = criterio switch
            {
                "Mascota" => asc ? query.OrderBy(c => c.NombreMascota) : query.OrderByDescending(c => c.NombreMascota),
                "Veterinario" => asc ? query.OrderBy(c => c.NombreVeterinario) : query.OrderByDescending(c => c.NombreVeterinario),
                "Estado" => asc ? query.OrderBy(c => c.Estado) : query.OrderByDescending(c => c.Estado),
                _ => asc ? query.OrderBy(c => c.FechaHora) : query.OrderByDescending(c => c.FechaHora),
            };

            // Asigna resultado final al DataGrid
            dgCitas.ItemsSource = query.ToList();
        }

        /// <summary>
        /// Lógica de seguridad para editar citas (solo si cumple condiciones)
        /// </summary>
        private void BtnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
            {
                // Rol admin o recepcionista
                bool esAdminORecepcionista = Sesion.UsuarioActual!.IdRol == 1 || Sesion.UsuarioActual.IdRol == 3;

                // Es su propia cita
                bool esSuCita = c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario;

                // Validación de permisos
                if (!(c.Estado == "Pendiente" && (esAdminORecepcionista || esSuCita)))
                {
                    MessageBox.Show("No tienes permisos para editar esta cita.");
                    return;
                }

                // Abre ventana de edición
                if (new WindowCita(c) { Owner = Window.GetWindow(this) }.ShowDialog() == true)
                    CargarDatos();
            }
        }

        /// <summary>
        /// Evento de cambio en filtros
        /// </summary>
        private void FiltroCita_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros de búsqueda
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaPaciente.Clear();
            txtBuscaVeterinario.Clear();
            txtBuscaDueno.Clear();

            cbBuscaEstado.SelectedIndex = 0;
            dtpFechaDesde.SelectedDate = null;
            dtpFechaHasta.SelectedDate = null;

            ActualizarTabla();
        }

        /// <summary>
        /// Crear nueva cita
        /// </summary>
        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            if (new WindowCita { Owner = Window.GetWindow(this) }.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Ver detalle desde botón
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
                AbrirVentanaDetalles(c.IdCita);
        }

        /// <summary>
        /// Ver detalle con doble click en la tabla
        /// </summary>
        private void DgCitas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCitas.SelectedItem is Cita c)
                AbrirVentanaDetalles(c.IdCita);
        }

        /// <summary>
        /// Abre ventana de ficha de cita
        /// </summary>
        private void AbrirVentanaDetalles(int idCita)
        {
            WindowFichaCita ficha = new(idCita)
            {
                Owner = Window.GetWindow(this)
            };

            ficha.ShowDialog();

            // Recarga datos tras cerrar
            CargarDatos();
        }
    }
}