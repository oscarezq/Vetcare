using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Usuarios;

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

        private UsuarioService _usuarioService = new UsuarioService();

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
            // 1. Instanciamos la ventana normal (el constructor sin nada)
            WindowUsuario win = new WindowUsuario();

            // 2. Buscamos el objeto "Veterinario" dentro del ComboBox de la ventana
            // Buscamos en la lista de roles que ya cargó la ventana
            foreach (Rol item in win.cbRol.Items)
            {
                if (item.NombreRol == "Veterinario")
                {
                    win.cbRol.SelectedItem = item; // Lo seleccionamos
                    break;
                }
            }

            // 3. Bloqueamos el ComboBox para que no lo cambien
            win.cbRol.IsEnabled = false;

            // 4. Cambiamos el título para que quede más pro
            win.lblTitulo.Text = "NUEVO VETERINARIO";

            // 5. La mostramos
            if (win.ShowDialog() == true)
            {
                CargarDatos(); // Refrescas tu lista de veterinarios
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Obtenemos el veterinario seleccionado del botón
                Button boton = sender as Button;
                Veterinario v = boton.DataContext as Veterinario;

                if (v != null)
                {
                    // 2. Necesitamos el objeto "Usuario" completo para la ventana.
                    // Lo obtenemos a través de su ID de usuario.
                    Usuario usu = _usuarioService.ObtenerPorId(v.IdUsuario);

                    if (usu != null)
                    {
                        // 3. Instanciamos la ventana pasándole el usuario (modo edición)
                        WindowUsuario win = new WindowUsuario(usu);

                        // 4. Bloqueamos los campos que no deben cambiarse en edición de veterinarios
                        win.cbRol.IsEnabled = false;    // El rol no se cambia desde aquí
                        win.txtUsername.IsEnabled = false; // El username suele ser fijo

                        // 5. Cambiamos el título
                        win.lblTitulo.Text = "EDITAR VETERINARIO";

                        // 6. Mostramos la ventana
                        if (win.ShowDialog() == true)
                        {
                            CargarDatos(); // Refrescamos el DataGrid
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario vinculado a este veterinario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir la edición: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void hlUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Veterinario v)
            {
                // Abrir la ficha pasando el IdUsuario
                WindowFichaUsuario ficha = new WindowFichaUsuario(v.IdUsuario);
                ficha.Owner = Window.GetWindow(this); // Establece ventana padre
                ficha.ShowDialog();

                // Aquí podrías recargar la lista si cambió algo
                CargarDatos();
            }
        }

        private void dgVeterinarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgVeterinarios.SelectedItem is Veterinario v)
            {
                abrirVentanaDetalles(v.IdVeterinario);
            }
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (dgVeterinarios.SelectedItem is Veterinario v)
            {
                abrirVentanaDetalles(v.IdVeterinario);
            }
        }

        private void abrirVentanaDetalles(int idVeterinario)
        {
            WindowFichaUsuario ficha = new WindowFichaUsuario(idVeterinario);
            ficha.Owner = Window.GetWindow(this);
            ficha.ShowDialog();

            CargarDatos(); // refrescar lista si hubo cambios
        }
    }
}