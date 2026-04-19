using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado y operaciones
    /// sobre las citas del sistema.
    /// Permite listar, filtrar, ordenar, crear, editar y ver detalles de citas.
    /// </summary>
    public partial class PageCitas : Page
    {
        // Lista completa de citas cargadas desde la base de datos.
        private List<Cita> listaCompleta = new();

        // Servicio de negocio para la gestión de citas.
        private readonly CitaService cs = new();

        /// <summary>
        /// Constructor de la página de citas.
        /// Inicializa la vista y configura los permisos según el rol del usuario.
        /// </summary>
        public PageCitas()
        {
            InitializeComponent();

            // Por defecto, se selecciona "Mis citas" 
            rbMisCitas.IsChecked = true;

            CargarDatos();
        }

        /// <summary>
        /// Carga todas las citas desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerTodas();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Actualiza la tabla aplicando filtros, búsquedas y ordenación.
        /// </summary>
        private void ActualizarTabla()
        {
            // Verificación para evitar errores durante la inicialización de componentes
            if (listaCompleta == null || dgCitas == null) 
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por rol (veterinario solo ve sus citas si tiene seleccionado 'Mis citas')
            if (Sesion.UsuarioActual!.IdRol != 1 && rbMisCitas.IsChecked == true)
                filtrado = filtrado.Where(c => c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario);

            // Filtro por nombre de la mascota
            if (!string.IsNullOrEmpty(txtBuscaPaciente.Text))
                filtrado = filtrado.Where(c => c.NombreMascota!.ToLower().Contains(txtBuscaPaciente.Text.ToLower()));

            // Filtro por nombre del veterinario
            if (!string.IsNullOrEmpty(txtBuscaVeterinario.Text))
                filtrado = filtrado.Where(c => c.NombreVeterinario!.ToLower().Contains(txtBuscaVeterinario.Text.ToLower()));

            // Filtro por nombre del dueño
            if (!string.IsNullOrEmpty(txtBuscaDueno.Text))
                filtrado = filtrado.Where(c => c.NombreDueno!.ToLower().Contains(txtBuscaDueno.Text.ToLower()));

            // Filtro por estado de la cita
            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
                filtrado = filtrado.Where(c => c.Estado == item.Content.ToString());

            // Filtro por fecha (desde)
            if (dtpFechaDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaHora.Date >= dtpFechaDesde.SelectedDate.Value.Date);

            // Filtro por fecha (hasta)
            if (dtpFechaHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaHora.Date <= dtpFechaHasta.SelectedDate.Value.Date);

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Fecha"; // Campo por el que ordenar
            bool asc = rbAsc.IsChecked == true; // Orden de ordenación

            filtrado = criterio switch
            {
                "Mascota" => asc ? filtrado.OrderBy(c => c.NombreMascota) : filtrado.OrderByDescending(c => c.NombreMascota),
                "Veterinario" => asc ? filtrado.OrderBy(c => c.NombreVeterinario) : filtrado.OrderByDescending(c => c.NombreVeterinario),
                "Estado" => asc ? filtrado.OrderBy(c => c.Estado) : filtrado.OrderByDescending(c => c.Estado),
                _ => asc ? filtrado.OrderBy(c => c.FechaHora) : filtrado.OrderByDescending(c => c.FechaHora),
            };

            dgCitas.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro.
        /// </summary>
        private void FiltroCita_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados en la vista.
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
        /// Controla la visibilidad del botón de edición según el rol del usuario
        /// y si la cita pertenece al veterinario actual.
        /// </summary>
        private void BtnEditarAccion_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
            {
                bool esAdminORecepcionista = Sesion.UsuarioActual!.IdRol == 1 || Sesion.UsuarioActual.IdRol == 3;
                bool esSuPropiaCita = c.IdVeterinario == Sesion.UsuarioActual.IdVeterinario;

                // Si el usuario es admin o recepcionista --> Botón de editar cita visible en todas las citas
                // Si el usuario es veterinario --> Botón de editar cita visible SOLO EN SUS CITAS
                btn.Visibility = (esAdminORecepcionista || esSuPropiaCita)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Abre la ventana para crear una nueva cita.
        /// </summary>
        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            WindowCita ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();

        }

        /// <summary>
        /// Abre la ventana para editar una cita existente.
        /// </summary>
        private void BtnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita cita)
            {
                WindowCita ventana = new(cita)
                {
                    Owner = Window.GetWindow(this)
                };

                if (ventana.ShowDialog() == true)
                    CargarDatos();
            }
        }

        /// <summary>
        /// Abre la ventana de detalles de una cita.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita c)
                AbrirVentanaDetalles(c.IdCita);
        }

        /// <summary>
        /// Abre la ficha de la cita al hacer doble clic en la tabla.
        /// </summary>
        private void DgCitas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCitas.SelectedItem is Cita c)
                AbrirVentanaDetalles(c.IdCita);
        }

        /// <summary>
        /// Abre la ventana de ficha de cita.
        /// </summary>
        /// <param name="idCita">ID de la cita a mostrar.</param>
        private void AbrirVentanaDetalles(int idCita)
        {
            WindowFichaCita ficha = new(idCita)
            {
                Owner = Window.GetWindow(this)
            };

            ficha.ShowDialog();
            CargarDatos();
        }
    }
}