using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Mascotas;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Página encargada de mostrar, filtrar, ordenar y gestionar las mascotas registradas en el sistema.
    /// </summary>
    public partial class PageMascotas : Page
    {
        // Lista que almacena todas las mascotas de la tabla mascotas de la base de datos vetcare
        private List<Mascota> listaCompleta = new List<Mascota>();

        // Servicio de la capa de negocio para la gestión de operaciones de mascotas.
        MascotaService ms = new MascotaService();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageMascotas"/>.
        /// </summary>
        public PageMascotas()
        {
            InitializeComponent();

            // Mostrar los datos en la tabla
            CargarDatos();
        }

        /// <summary>
        /// Recupera la información de las mascotas desde la Capa de Negocio.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Invocación al servicio para obtener la colección completa
                listaCompleta = ms.ObtenerTodas();

                // Refresca la UI con los datos obtenidos
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar mascotas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método que procesa la lista de mascotas aplicando los filtros de texto, rangos numéricos y los criterios de ordenación.
        /// </summary>
        private void ActualizarTabla()
        {
            try
            {
                // Verificamos que la lista original no sea nula
                if (listaCompleta == null)
                    return;

                // Verificamos que los controles de la interfaz existan para evitar errores al iniciar
                if (rbAsc == null || cbOrdenarPor == null || dgMascotas == null)
                    return;

                // FILTRADO
                List<Mascota> listaFiltrada = new List<Mascota>();

                string estadoBusca = "";
                if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado)
                {
                    estadoBusca = itemEstado.Content.ToString();
                }

                // Recorremos todas las mascotas de la lista completa
                foreach (Mascota m in listaCompleta)
                {
                    // Convertimos a minúsculas los campos de texto
                    string nombreBusca = txtBuscaNombre.Text.ToLower();
                    string numeroChipBusca = txtBuscaNumeroChip.Text.ToLower();
                    string especieBusca = txtBuscaEspecie.Text.ToLower();
                    string razaBusca = txtBuscaRaza.Text.ToLower();
                    string sexoBusca = "";
                    ComboBoxItem itemSexo = (ComboBoxItem)cbBuscaSexo.SelectedItem;
                    if (itemSexo != null && itemSexo.Content != null)
                    {
                        sexoBusca = itemSexo.Content.ToString().ToLower();
                    }

                    // Comprobamos cada filtro de texto
                    if (!string.IsNullOrEmpty(nombreBusca) && !m.Nombre.ToLower().Contains(nombreBusca)) continue;
                    if (!string.IsNullOrEmpty(numeroChipBusca) && !m.NumeroChip.ToLower().Contains(numeroChipBusca)) continue;
                    if (!string.IsNullOrEmpty(especieBusca) && !m.NombreEspecie.ToLower().Contains(especieBusca)) continue;
                    if (!string.IsNullOrEmpty(razaBusca) && !m.NombreRaza.ToLower().Contains(razaBusca)) continue;
                    if (sexoBusca != "" && sexoBusca != "todos")
                    {
                        if (m.Sexo == null || !m.Sexo.ToLower().Contains(sexoBusca)) continue;
                    }
                    if (estadoBusca != "Todos")
                    {
                        bool buscarActivos = (estadoBusca == "Activo");
                        // Si la mascota no coincide con el estado buscado, saltar
                        if (m.Activo != buscarActivos) continue;
                    }

                    // Filtro para la Fecha de Nacimiento (Desde - Hasta)
                    if (dtpBuscaFechaDesde.SelectedDate.HasValue && m.FechaNacimiento.Date < dtpBuscaFechaDesde.SelectedDate.Value.Date) continue;
                    if (dtpBuscaFechaHasta.SelectedDate.HasValue && m.FechaNacimiento.Date > dtpBuscaFechaHasta.SelectedDate.Value.Date) continue;

                    // Si cumple todo, se añade
                    listaFiltrada.Add(m);
                }

                // ORDENACIÓN
                ComboBoxItem itemSeleccionado = (ComboBoxItem)cbOrdenarPor.SelectedItem;

                if (itemSeleccionado != null && dgMascotas != null)
                {
                    string criterio = itemSeleccionado.Content.ToString();
                    bool esAscendente = (bool)rbAsc.IsChecked;

                    switch (criterio)
                    {
                        case "Nombre":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Nombre.CompareTo(y.Nombre) : y.Nombre.CompareTo(x.Nombre));
                            break;
                        case "Especie":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreEspecie.CompareTo(y.NombreEspecie) : y.NombreEspecie.CompareTo(x.NombreEspecie));
                            break;
                        case "Raza":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreRaza.CompareTo(y.NombreRaza) : y.NombreRaza.CompareTo(x.NombreRaza));
                            break;
                        case "Sexo":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Sexo.CompareTo(y.Sexo) : y.Sexo.CompareTo(x.Sexo));
                            break;
                        case "Peso":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Peso.CompareTo(y.Peso) : y.Peso.CompareTo(x.Peso));
                            break;
                        case "Dueño":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreDueno.CompareTo(y.NombreDueno) : y.NombreDueno.CompareTo(x.NombreDueno));
                            break;
                        case "Fecha de Nacimiento":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.FechaNacimiento.CompareTo(y.FechaNacimiento) : y.FechaNacimiento.CompareTo(x.FechaNacimiento));
                            break;
                    }
                }

                // Enviamos la lista resultante a la tabla
                dgMascotas.ItemsSource = listaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar tabla: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- EVENTOS DE INTERFAZ ---

        private void FiltroAvanzado_Changed(object sender, TextChangedEventArgs e) => ActualizarTabla();
        private void dtpBuscaFechaDesde_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void dtpBuscaFechaHasta_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void cbOrdenarPor_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void cbSexo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void OrdenDirection_Checked(object sender, RoutedEventArgs e) => ActualizarTabla();
        private void cbBuscaEstado_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();


        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Clear();
            txtBuscaEspecie.Clear();
            txtBuscaRaza.Clear();
            cbBuscaSexo.SelectedIndex = 0;
            dtpBuscaFechaDesde.SelectedDate = null;
            dtpBuscaFechaHasta.SelectedDate = null;
            cbOrdenarPor.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

        // --- ACCIONES CRUD ---

        private void btnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowMascota ventanaMascota = new WindowMascota();
                ventanaMascota.ShowDialog();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al añadir mascota: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button botonPulsado = sender as Button;
                Mascota mascotaDeLaFila = botonPulsado.DataContext as Mascota;

                if (mascotaDeLaFila != null)
                {
                    WindowMascota ventanaEdicion = new WindowMascota(mascotaDeLaFila);
                    ventanaEdicion.ShowDialog();
                    CargarDatos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar mascota: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button botonPulsado = sender as Button;
                Mascota mascotaDeLaFila = botonPulsado.DataContext as Mascota;

                if (mascotaDeLaFila != null)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Dar de baja a {mascotaDeLaFila.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (ms.Desactivar(mascotaDeLaFila.IdMascota))
                        {
                            MessageBox.Show("Mascota dada de baja correctamente.", "Información",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            CargarDatos();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al dar de baja mascota: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEliminarVarios_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgMascotas.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Debes seleccionar al menos una mascota.",
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Dar de baja {dgMascotas.SelectedItems.Count} mascota(s)?",
                    "Confirmar acción",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    List<int> ids = new List<int>();

                    foreach (Mascota m in dgMascotas.SelectedItems)
                    {
                        ids.Add(m.IdMascota);
                    }

                    if (ms.DesactivarVarios(ids))
                    {
                        MessageBox.Show("Mascotas dadas de baja correctamente.",
                            "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                        CargarDatos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Ver ficha de la MASCOTA
        private void btnVerFichaMascota_Click(object sender, RoutedEventArgs e)
        {
            var mascotaSeleccionada = (sender as Hyperlink).DataContext as Mascota;
            if (mascotaSeleccionada != null)
            {
                WindowFichaMascota win = new WindowFichaMascota(mascotaSeleccionada.IdMascota);
                win.Owner = Window.GetWindow(this); // Para que se centre respecto a la principal
                win.ShowDialog();

                // Recargar grid si hubo cambios
                CargarDatos();
            }
        }

        // Ver ficha del CLIENTE (Dueño)
        private void btnVerFichaCliente_Click(object sender, RoutedEventArgs e)
        {
            var mascotaSeleccionada = (sender as Hyperlink).DataContext as Mascota;
            if (mascotaSeleccionada != null)
            {
                WindowFichaCliente win = new WindowFichaCliente(mascotaSeleccionada.IdCliente);
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();

                // Recargar grid si hubo cambios
                CargarDatos();
            }
        }

        private void dgMascotas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgMascotas.SelectedItem is Mascota mascotaSeleccionada)
            {
                abrirVentanaDetalles(mascotaSeleccionada.IdMascota);
            }
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            Button botonPulsado = sender as Button;
            Mascota mascotaDeLaFila = botonPulsado.DataContext as Mascota;

            if (mascotaDeLaFila != null)
            {
                abrirVentanaDetalles(mascotaDeLaFila.IdMascota);
            }
        }

        private void abrirVentanaDetalles(int idMascota)
        {
            WindowFichaMascota win = new WindowFichaMascota(idMascota);
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();

            CargarDatos();
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button botonPulsado = sender as Button;
                Mascota mascota = botonPulsado.DataContext as Mascota;

                if (mascota == null) return;

                ClienteService cs = new ClienteService();

                Cliente cliente = cs.ObtenerPorId(mascota.IdCliente);
                bool clienteActivo = cliente.Activo;

                // 🔴 CASO: Cliente inactivo
                if (!clienteActivo)
                {
                    MessageBoxResult resultado = MessageBox.Show(
                        $"El dueño de {mascota.Nombre} está inactivo.\n\n" +
                        "¿Deseas activarlo también?",
                        "Dueño inactivo",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (resultado == MessageBoxResult.Yes)
                    {
                        // Activar cliente
                        if (!cs.Reactivar(mascota.IdCliente))
                        {
                            MessageBox.Show("No se pudo reactivar el dueño.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else if (resultado == MessageBoxResult.No)
                    {
                        // Solo cancelar operación
                        return;
                    }
                }

                // ✅ Reactivar mascota
                MessageBoxResult confirmacion = MessageBox.Show(
                    $"¿Deseas reactivar a {mascota.Nombre}?",
                    "Confirmar acción",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes)
                {
                    if (ms.Reactivar(mascota.IdMascota))
                    {
                        MessageBox.Show("Mascota reactivada correctamente.",
                            "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo reactivar la mascota.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar mascota: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}