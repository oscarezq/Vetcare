using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Citas
{
    public partial class PageCitas : Page
    {
        private List<Cita> listaCompleta = new List<Cita>();
        CitaService cs = new CitaService();

        public PageCitas()
        {
            InitializeComponent();

            // Ocultar selector si es Admin
            if (Sesion.UsuarioActual.IdRol == 1)
            {
                brdSelectorVista.Visibility = Visibility.Collapsed;
                rbTodasCitas.IsChecked = true; // El admin siempre ve todas
            }
            else
            {
                rbMisCitas.IsChecked = true; // El veterinario empieza viendo las suyas
            }

            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerTodas();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarTabla()
        {
            if (listaCompleta == null || dgCitas == null) return;

            // --- FILTRADO CON LINQ ---
            var query = listaCompleta.AsEnumerable();

            // 1. Filtro de Pestaña/Selector (Solo si no es admin)
            if (Sesion.UsuarioActual.IdRol != 1 && rbMisCitas.IsChecked == true)
            {
                query = query.Where(c => c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario);
            }

            // 2. Filtros de Búsqueda (Tus filtros originales)
            if (!string.IsNullOrEmpty(txtBuscaPaciente.Text))
                query = query.Where(c => c.NombreMascota.ToLower().Contains(txtBuscaPaciente.Text.ToLower()));

            if (!string.IsNullOrEmpty(txtBuscaVeterinario.Text))
                query = query.Where(c => c.NombreVeterinario.ToLower().Contains(txtBuscaVeterinario.Text.ToLower()));

            if (!string.IsNullOrEmpty(txtBuscaDueno.Text))
                query = query.Where(c => c.NombreDueno.ToLower().Contains(txtBuscaDueno.Text.ToLower()));

            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
                query = query.Where(c => c.Estado == item.Content.ToString());

            // 3. Filtros de Fecha
            if (dtpFechaDesde.SelectedDate.HasValue)
                query = query.Where(c => c.FechaHora.Date >= dtpFechaDesde.SelectedDate.Value.Date);
            if (dtpFechaHasta.SelectedDate.HasValue)
                query = query.Where(c => c.FechaHora.Date <= dtpFechaHasta.SelectedDate.Value.Date);

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Fecha";
            bool asc = rbAsc.IsChecked == true;

            switch (criterio)
            {
                case "Mascota": query = asc ? query.OrderBy(c => c.NombreMascota) : query.OrderByDescending(c => c.NombreMascota); break;
                case "Veterinario": query = asc ? query.OrderBy(c => c.NombreVeterinario) : query.OrderByDescending(c => c.NombreVeterinario); break;
                case "Estado": query = asc ? query.OrderBy(c => c.Estado) : query.OrderByDescending(c => c.Estado); break;
                default: query = asc ? query.OrderBy(c => c.FechaHora) : query.OrderByDescending(c => c.FechaHora); break;
            }

            dgCitas.ItemsSource = query.ToList();
        }

        // LÓGICA DE SEGURIDAD PARA EL LÁPIZ (Sin tocar la entidad Cita)
        private void btnEditarAccion_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
            {
                // Admin (1) edita todo. Veterinario solo lo suyo.
                btn.Visibility = (Sesion.UsuarioActual.IdRol == 1 || c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario)
                                 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void FiltroCita_Changed(object sender, EventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaPaciente.Clear(); txtBuscaVeterinario.Clear(); txtBuscaDueno.Clear();
            cbBuscaEstado.SelectedIndex = 0; dtpFechaDesde.SelectedDate = null; dtpFechaHasta.SelectedDate = null;
            ActualizarTabla();
        }

        private void btnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            if (new WindowCita { Owner = Window.GetWindow(this) }.ShowDialog() == true) CargarDatos();
        }

        private void btnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
                if (new WindowCita(c) { Owner = Window.GetWindow(this) }.ShowDialog() == true) CargarDatos();
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c) abrirVentanaDetalles(c.IdCita);
        }

        private void dgCitas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCitas.SelectedItem is Cita c) abrirVentanaDetalles(c.IdCita);
        }

        private void abrirVentanaDetalles(int idCita)
        {
            WindowFichaCita ficha = new WindowFichaCita(idCita) { Owner = Window.GetWindow(this) };
            ficha.ShowDialog();
            CargarDatos();
        }
    }
}