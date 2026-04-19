using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Usuarios
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado,
    /// ordenación y operaciones sobre los usuarios del sistema.
    /// Permite listar, buscar, ordenar, crear, editar, activar/desactivar y ver detalles de usuarios.
    /// </summary>
    public partial class PageUsuarios : Page
    {
        // Lista completa de usuarios cargados desde la base de datos.
        private List<Usuario> listaCompleta = new();

        // Servicio de negocio para la gestión de usuarios.
        private readonly UsuarioService us = new();

        /// <summary>
        /// Constructor de la página de usuarios.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageUsuarios()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los usuarios desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = us.ObtenerTodos() ?? new List<Usuario>();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}",
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
            // Verificación para evitar errores durante la inicialización
            if (listaCompleta == null || dgUsuarios == null)
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por username
            if (!string.IsNullOrWhiteSpace(txtBuscaUsername.Text))
                filtrado = filtrado.Where(u => u.Username!.ToLower().Contains(txtBuscaUsername.Text.ToLower()));

            // Filtro por nombre completo
            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
                filtrado = filtrado.Where(u => u.NombreCompleto!.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

            // Filtro por rol
            if (cbBuscaRol.SelectedItem is ComboBoxItem itemRol &&
                itemRol.Content.ToString() != "Todos")
            {
                filtrado = filtrado.Where(u => u.NombreRol == itemRol.Content.ToString());
            }

            // Filtro por estado (activo/inactivo)
            if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado)
            {
                string estado = itemEstado.Content.ToString()!;

                if (estado == "Activo")
                    filtrado = filtrado.Where(u => u.Activo);
                else if (estado == "Inactivo")
                    filtrado = filtrado.Where(u => !u.Activo);
            }

            // Filtro por fecha (desde)
            if (dpBuscaFechaDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(u =>
                    u.FechaAlta.Date >= dpBuscaFechaDesde.SelectedDate.Value.Date);

            // Filtro por fecha (hasta)
            if (dpBuscaFechaHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(u =>
                    u.FechaAlta.Date <= dpBuscaFechaHasta.SelectedDate.Value.Date);

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Username";
            bool asc = rbAsc.IsChecked == true;

            filtrado = criterio switch
            {
                "Username" => asc ? filtrado.OrderBy(u => u.Username) : filtrado.OrderByDescending(u => u.Username),
                "Nombre" => asc ? filtrado.OrderBy(u => u.NombreCompleto) : filtrado.OrderByDescending(u => u.NombreCompleto),
                "Rol" => asc ? filtrado.OrderBy(u => u.NombreRol) : filtrado.OrderByDescending(u => u.NombreRol),
                _ => asc ? filtrado.OrderBy(u => u.FechaAlta) : filtrado.OrderByDescending(u => u.FechaAlta),
            };

            dgUsuarios.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro.
        /// </summary>
        private void FiltroUsuario_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaUsername.Clear();
            txtBuscaNombre.Clear();

            dpBuscaFechaDesde.SelectedDate = null;
            dpBuscaFechaHasta.SelectedDate = null;

            cbBuscaRol.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;

            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo usuario.
        /// </summary>
        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            WindowUsuario win = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (win.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar un usuario existente.
        /// </summary>
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                WindowUsuario win = new(u)
                {
                    Owner = Window.GetWindow(this)
                };

                if (win.ShowDialog() == true)
                    CargarDatos();
            }
        }

        /// <summary>
        /// Activa o desactiva un usuario.
        /// </summary>
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                string accion = u.Activo ? "desactivar" : "activar";

                var confirmacion = MessageBox.Show(
                    $"¿Desea {accion} al usuario {u.Username}?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes && us.Eliminar(u.IdUsuario))
                    CargarDatos();
            }
        }

        /// <summary>
        /// Abre la ficha de un usuario.
        /// </summary>
        private void HyperlinkUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Usuario u)
                AbrirFichaUsuario(u.IdUsuario);
        }

        /// <summary>
        /// Botón para ver detalles de un usuario.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
                AbrirFichaUsuario(u.IdUsuario);
        }

        /// <summary>
        /// Abre la ficha del usuario al hacer doble clic en la tabla.
        /// </summary>
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario u)
                AbrirFichaUsuario(u.IdUsuario);
        }

        /// <summary>
        /// Abre la ventana de ficha de usuario.
        /// </summary>
        private void AbrirFichaUsuario(int idUsuario)
        {
            WindowFichaUsuario ficha = new(idUsuario)
            {
                Owner = Window.GetWindow(this)
            };

            ficha.ShowDialog();
            CargarDatos();
        }

        /// <summary>
        /// Reactiva un usuario previamente desactivado.
        /// </summary>
        private void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Usuario u)
                {
                    var confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar a {u.NombreCompleto}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (us.Reactivar(u.IdUsuario))
                        {
                            MessageBox.Show("Usuario reactivado correctamente.",
                                "Información",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo reactivar el usuario.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar el usuario: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}