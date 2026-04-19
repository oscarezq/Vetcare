using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Mascotas;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado,
    /// ordenación y operaciones CRUD sobre las mascotas del sistema.
    /// Permite listar, buscar, ordenar, crear, editar, eliminar y ver detalles de mascotas.
    /// </summary>
    public partial class PageMascotas : Page
    {
        // Lista completa de mascotas cargadas desde la base de datos.
        private List<Mascota> listaCompleta = new();

        // Servicio de negocio para la gestión de mascotas.
        private readonly MascotaService ms = new();

        /// <summary>
        /// Constructor de la página de mascotas.
        /// Inicializa la vista y carga los datos iniciales.
        /// </summary>
        public PageMascotas()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todas las mascotas desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = ms.ObtenerTodas();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar mascotas: {ex.Message}",
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
            if (listaCompleta == null || dgMascotas == null)
                return;

            try
            {
                // --- FILTRADO ---
                var filtrado = listaCompleta.AsEnumerable();

                // Filtro por nombre
                if (!string.IsNullOrEmpty(txtBuscaNombre.Text))
                    filtrado = filtrado.Where(m => m.Nombre!.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

                // Filtro por número de chip
                if (!string.IsNullOrEmpty(txtBuscaNumeroChip.Text))
                    filtrado = filtrado.Where(m => m.NumeroChip!.ToLower().Contains(txtBuscaNumeroChip.Text.ToLower()));

                // Filtro por especie
                if (!string.IsNullOrEmpty(txtBuscaEspecie.Text))
                    filtrado = filtrado.Where(m => m.NombreEspecie!.ToLower().Contains(txtBuscaEspecie.Text.ToLower()));

                // Filtro por raza
                if (!string.IsNullOrEmpty(txtBuscaRaza.Text))
                    filtrado = filtrado.Where(m => m.NombreRaza!.ToLower().Contains(txtBuscaRaza.Text.ToLower()));

                // Filtro por sexo
                if (cbBuscaSexo.SelectedItem is ComboBoxItem itemSexo &&
                    itemSexo.Content.ToString() != "Todos")
                {
                    filtrado = filtrado.Where(m =>
                        m.Sexo != null &&
                        m.Sexo.ToLower().Contains(itemSexo.Content.ToString()!.ToLower()));
                }

                // Filtro por estado (activo/inactivo)
                if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado &&
                    itemEstado.Content.ToString() != "Todos")
                {
                    bool activos = itemEstado.Content.ToString() == "Activo";
                    filtrado = filtrado.Where(m => m.Activo == activos);
                }

                // Filtro por fecha (desde)
                if (dtpBuscaFechaDesde.SelectedDate.HasValue)
                    filtrado = filtrado.Where(m =>
                        m.FechaNacimiento.Date >= dtpBuscaFechaDesde.SelectedDate.Value.Date);

                // Filtro por fecha (hasta)
                if (dtpBuscaFechaHasta.SelectedDate.HasValue)
                    filtrado = filtrado.Where(m =>
                        m.FechaNacimiento.Date <= dtpBuscaFechaHasta.SelectedDate.Value.Date);

                // --- ORDENACIÓN ---
                string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Nombre";
                bool asc = rbAsc.IsChecked == true;

                filtrado = criterio switch
                {
                    "Nombre" => asc ? filtrado.OrderBy(m => m.Nombre) : filtrado.OrderByDescending(m => m.Nombre),
                    "Especie" => asc ? filtrado.OrderBy(m => m.NombreEspecie) : filtrado.OrderByDescending(m => m.NombreEspecie),
                    "Raza" => asc ? filtrado.OrderBy(m => m.NombreRaza) : filtrado.OrderByDescending(m => m.NombreRaza),
                    "Sexo" => asc ? filtrado.OrderBy(m => m.Sexo) : filtrado.OrderByDescending(m => m.Sexo),
                    "Peso" => asc ? filtrado.OrderBy(m => m.Peso) : filtrado.OrderByDescending(m => m.Peso),
                    "Dueño" => asc ? filtrado.OrderBy(m => m.NombreDueno) : filtrado.OrderByDescending(m => m.NombreDueno),
                    _ => asc ? filtrado.OrderBy(m => m.FechaNacimiento) : filtrado.OrderByDescending(m => m.FechaNacimiento),
                };

                dgMascotas.ItemsSource = filtrado.ToList();
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
        /// Evento que se ejecuta cuando cambia algún filtro de texto.
        /// </summary>
        private void FiltroAvanzado_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNombre.Clear();
            txtBuscaNumeroChip.Clear();
            txtBuscaEspecie.Clear();
            txtBuscaRaza.Clear();

            cbBuscaSexo.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;

            dtpBuscaFechaDesde.SelectedDate = null;
            dtpBuscaFechaHasta.SelectedDate = null;

            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva mascota.
        /// </summary>
        private void BtnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowMascota ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            ventana.ShowDialog();
            CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar una mascota existente.
        /// </summary>
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Mascota m)
            {
                WindowMascota ventana = new(m)
                {
                    Owner = Window.GetWindow(this)
                };

                ventana.ShowDialog();
                CargarDatos();
            }
        }

        /// <summary>
        /// Da de baja (desactiva) una mascota.
        /// </summary>
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Mascota m)
            {
                var confirmacion = MessageBox.Show(
                    $"¿Dar de baja a {m.Nombre}?",
                    "Confirmar acción",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmacion == MessageBoxResult.Yes && ms.Desactivar(m.IdMascota))
                {
                    MessageBox.Show("Mascota dada de baja correctamente.",
                        "Información",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    CargarDatos();
                }
            }
        }

        /// <summary>
        /// Abre la ficha de una mascota.
        /// </summary>
        private void BtnVerFichaMascota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink { DataContext: Mascota m })
                AbrirVentanaDetalles(m.IdMascota);
        }


        /// <summary>
        /// Abre la ficha de la mascota al hacer doble clic en la tabla.
        /// </summary>
        private void DgMascotas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgMascotas.SelectedItem is Mascota m)
                AbrirVentanaDetalles(m.IdMascota);
        }

        /// <summary>
        /// Botón para ver detalles de una mascota.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Mascota m)
                AbrirVentanaDetalles(m.IdMascota);
        }

        /// <summary>
        /// Abre la ventana de ficha de mascota.
        /// </summary>
        private void AbrirVentanaDetalles(int idMascota)
        {
            WindowFichaMascota win = new(idMascota)
            {
                Owner = Window.GetWindow(this)
            };

            win.ShowDialog();
            CargarDatos();
        }

        /// <summary>
        /// Reactiva una mascota previamente dada de baja.
        /// Valida que el cliente esté activo antes de permitir la acción.
        /// </summary>
        private void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Mascota m)
            {
                ClienteService cs = new();
                Cliente? cliente = cs.ObtenerPorId(m.IdCliente);

                // Si el cliente está inactivo, se solicita reactivarlo primero
                if (cliente != null && !cliente.Activo)
                {
                    var resultado = MessageBox.Show(
                        $"El dueño ({cliente.NombreCompleto}) está inactivo.\n¿Deseas reactivarlo?",
                        "Requisito",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (resultado != MessageBoxResult.Yes || !cs.Reactivar(m.IdCliente))
                        return;
                }

                var confirmacion = MessageBox.Show(
                    $"¿Deseas reactivar a {m.Nombre}?",
                    "Confirmar acción",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion == MessageBoxResult.Yes && ms.Reactivar(m.IdMascota))
                {
                    MessageBox.Show("Mascota reactivada correctamente.",
                        "Información",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    CargarDatos();
                }
            }
        }
    }
}