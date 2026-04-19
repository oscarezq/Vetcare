using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Usuarios;

namespace Vetcare.Presentacion.Veterinarios
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado,
    /// ordenación y operaciones sobre los veterinarios del sistema.
    /// Permite listar, buscar, ordenar, crear, editar, eliminar y ver detalles de veterinarios.
    /// </summary>
    public partial class PageVeterinarios : Page
    {
        // Lista completa de veterinarios cargados desde la base de datos.
        private List<Veterinario> listaCompleta = new();

        // Servicio de negocio para la gestión de veterinarios.
        private readonly VeterinarioService vs = new();

        // Servicio de usuarios (necesario para edición).
        private readonly UsuarioService usuarioService = new();

        /// <summary>
        /// Constructor de la página de veterinarios.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageVeterinarios()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los veterinarios desde la base de datos.
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
                MessageBox.Show($"Error al cargar veterinarios: {ex.Message}",
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
            if (listaCompleta == null || dgVeterinarios == null)
                return;

            try
            {
                // --- FILTRADO ---
                var filtrado = listaCompleta.AsEnumerable();

                // Filtro por nombre
                if (!string.IsNullOrEmpty(txtBuscaNombre.Text))
                    filtrado = filtrado.Where(v => v.Nombre!.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

                // Filtro por apellidos
                if (!string.IsNullOrEmpty(txtBuscaApellidos.Text))
                    filtrado = filtrado.Where(v => v.Apellidos!.ToLower().Contains(txtBuscaApellidos.Text.ToLower()));

                // Filtro por especialidad
                if (!string.IsNullOrEmpty(txtBuscaEspecialidad.Text))
                    filtrado = filtrado.Where(v => v.Especialidad!.ToLower().Contains(txtBuscaEspecialidad.Text.ToLower()));

                // Filtro por número de colegiado
                if (!string.IsNullOrEmpty(txtBuscaNumeroColegiado.Text))
                    filtrado = filtrado.Where(v => v.NumeroColegiado!.ToLower().Contains(txtBuscaNumeroColegiado.Text.ToLower()));

                // --- ORDENACIÓN ---
                string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Nombre";
                bool asc = rbAsc.IsChecked == true;

                filtrado = criterio switch
                {
                    "Nombre" => asc ? filtrado.OrderBy(v => v.Nombre) : filtrado.OrderByDescending(v => v.Nombre),
                    "Apellidos" => asc ? filtrado.OrderBy(v => v.Apellidos) : filtrado.OrderByDescending(v => v.Apellidos),
                    "Especialidad" => asc ? filtrado.OrderBy(v => v.Especialidad) : filtrado.OrderByDescending(v => v.Especialidad),
                    _ => asc ? filtrado.OrderBy(v => v.NumeroColegiado) : filtrado.OrderByDescending(v => v.NumeroColegiado),
                };

                // Actualizamos el DataGrid
                dgVeterinarios.ItemsSource = filtrado.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar tabla: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // --- EVENTOS DE FILTROS ---

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro.
        /// </summary>
        private void FiltroAvanzado_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Clear();
            txtBuscaApellidos.Clear();
            txtBuscaEspecialidad.Clear();
            txtBuscaNumeroColegiado.Clear();

            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo veterinario.
        /// </summary>
        private void BtnNuevoVeterinario_Click(object sender, RoutedEventArgs e)
        {
            WindowUsuario win = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Seleccionamos automáticamente el rol "Veterinario"
            foreach (Rol item in win.cbRol.Items)
            {
                if (item.NombreRol == "Veterinario")
                {
                    win.cbRol.SelectedItem = item;
                    break;
                }
            }

            // Bloqueamos el rol y personalizamos la ventana
            win.cbRol.IsEnabled = false;
            win.lblTitulo.Text = "NUEVO VETERINARIO";

            if (win.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar un veterinario existente.
        /// </summary>
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Veterinario v)
                {
                    // Obtenemos el usuario asociado al veterinario
                    Usuario? usu = usuarioService.ObtenerPorId(v.IdUsuario);

                    if (usu == null)
                    {
                        MessageBox.Show("No se encontró el usuario vinculado.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    WindowUsuario win = new(usu)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    // Bloqueamos campos no editables
                    win.cbRol.IsEnabled = false;
                    win.txtUsername.IsEnabled = false;
                    win.lblTitulo.Text = "EDITAR VETERINARIO";

                    if (win.ShowDialog() == true)
                        CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir la edición: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Realiza un borrado lógico de un veterinario.
        /// </summary>
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Veterinario v)
                {
                    var confirmacion = MessageBox.Show(
                        $"¿Deseas eliminar a {v.Nombre} {v.Apellidos}?",
                        "Confirmar eliminación",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (confirmacion == MessageBoxResult.Yes && vs.BorradoLogico(v.IdVeterinario))
                    {
                        MessageBox.Show("Veterinario eliminado correctamente.",
                            "Información",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        CargarDatos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar veterinario: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre la ficha del usuario asociado al veterinario.
        /// </summary>
        private void HlUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Veterinario v)
            {
                WindowFichaUsuario ficha = new(v.IdUsuario)
                {
                    Owner = Window.GetWindow(this)
                };

                ficha.ShowDialog();
                CargarDatos();
            }
        }

        /// <summary>
        /// Abre la ficha del veterinario al hacer doble clic en la tabla.
        /// </summary>
        private void DgVeterinarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgVeterinarios.SelectedItem is Veterinario v)
                AbrirVentanaDetalles(v.IdVeterinario);
        }

        /// <summary>
        /// Botón para ver detalles de un veterinario.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (dgVeterinarios.SelectedItem is Veterinario v)
                AbrirVentanaDetalles(v.IdVeterinario);
        }

        /// <summary>
        /// Abre la ventana de ficha de veterinario.
        /// </summary>
        private void AbrirVentanaDetalles(int idVeterinario)
        {
            WindowFichaUsuario ficha = new(idVeterinario)
            {
                Owner = Window.GetWindow(this)
            };

            ficha.ShowDialog();
            CargarDatos();
        }
    }
}