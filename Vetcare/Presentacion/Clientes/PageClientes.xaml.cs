using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Página de presentación encargada de gestionar la visualización, filtrado y operaciones
    /// sobre los clientes del sistema.
    /// Permite listar, filtrar, ordenar, crear, editar, eliminar (desactivar) y reactivar clientes,
    /// así como acceder a su ficha detallada.
    /// </summary>
    public partial class PageClientes : Page
    {
        // Lista completa de clientes cargados desde la base de datos.
        private List<Cliente> listaCompleta = new();

        // Servicio de negocio para la gestión de clientes.
        private readonly ClienteService cs = new();

        /// <summary>
        /// Constructor de la página de clientes.
        /// Inicializa la vista y carga los datos.
        /// </summary>
        public PageClientes()
        {
            InitializeComponent();
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los clientes desde la base de datos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerTodos();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}",
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
            if (listaCompleta == null || dgClientes == null)
                return;

            // --- FILTRADO ---
            var filtrado = listaCompleta.AsEnumerable();

            // Filtro por número de documento
            if (!string.IsNullOrEmpty(txtBuscaNumDocumento.Text))
                filtrado = filtrado.Where(c => c.NumDocumento!.ToLower().Contains(txtBuscaNumDocumento.Text.ToLower()));

            // Filtro por nombre completo del cliente
            if (!string.IsNullOrEmpty(txtBuscaCliente.Text))
                filtrado = filtrado.Where(c => c.NombreCompleto!.ToLower().Contains(txtBuscaCliente.Text.ToLower()));

            // Filtro por teléfono
            if (!string.IsNullOrEmpty(txtBuscaTelefono.Text))
                filtrado = filtrado.Where(c => c.Telefono!.ToLower().Contains(txtBuscaTelefono.Text.ToLower()));

            // Filtro por email
            if (!string.IsNullOrEmpty(txtBuscaEmail.Text))
                filtrado = filtrado.Where(c => c.Email!.ToLower().Contains(txtBuscaEmail.Text.ToLower()));

            // Filtro por fecha de alta (desde)
            if (dtpBuscaFechaDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date >= dtpBuscaFechaDesde.SelectedDate.Value.Date);

            // Filtro por fecha de alta (hasta)
            if (dtpBuscaFechaHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date <= dtpBuscaFechaHasta.SelectedDate.Value.Date);

            // Filtro por estado
            if (cbBuscaEstado.SelectedItem is ComboBoxItem item && item.Content.ToString() != "Todos")
            {
                if (item.Content.ToString() == "Activo")
                    filtrado = filtrado.Where(c => c.Activo);
                else if (item.Content.ToString() == "Inactivo")
                    filtrado = filtrado.Where(c => !c.Activo);
            }

            // --- ORDENACIÓN ---
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Nombre";
            bool asc = rbAsc.IsChecked == true;

            filtrado = criterio switch
            {
                "DNI" => asc ? filtrado.OrderBy(c => c.NumDocumento) : filtrado.OrderByDescending(c => c.NumDocumento),
                "Nombre" => asc ? filtrado.OrderBy(c => c.Nombre) : filtrado.OrderByDescending(c => c.Nombre),
                "Apellidos" => asc ? filtrado.OrderBy(c => c.Apellidos) : filtrado.OrderByDescending(c => c.Apellidos),
                "Teléfono" => asc ? filtrado.OrderBy(c => c.Telefono) : filtrado.OrderByDescending(c => c.Telefono),
                "Email" => asc ? filtrado.OrderBy(c => c.Email) : filtrado.OrderByDescending(c => c.Email),
                _ => asc ? filtrado.OrderBy(c => c.FechaAlta) : filtrado.OrderByDescending(c => c.FechaAlta),
            };

            dgClientes.ItemsSource = filtrado.ToList();
        }

        /// <summary>
        /// Evento que se ejecuta cuando cambia algún filtro de texto.
        /// </summary>
        private void FiltroAvanzado_Changed(object sender, EventArgs e) => ActualizarTabla();

        /// <summary>
        /// Limpia todos los filtros aplicados en la vista.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNumDocumento.Clear();
            txtBuscaCliente.Clear();
            txtBuscaTelefono.Clear();
            txtBuscaEmail.Clear();

            dtpBuscaFechaDesde.SelectedDate = null;
            dtpBuscaFechaHasta.SelectedDate = null;

            cbOrdenarPor.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;

            rbAsc.IsChecked = true;

            ActualizarTabla();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo cliente.
        /// </summary>
        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente ventana = new() { 
                Owner = Window.GetWindow(this) 
            };

            if (ventana.ShowDialog() == true)
                CargarDatos();
        }

        /// <summary>
        /// Abre la ventana para editar un cliente existente.
        /// </summary>
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Cliente c)
            {
                WindowCliente ventana = new(c) { 
                    Owner = Window.GetWindow(this) 
                };

                if (ventana.ShowDialog() == true)
                    CargarDatos();
            }
        }

        /// <summary>
        /// Desactiva (da de baja) un cliente junto con sus mascotas asociadas.
        /// </summary>
        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Cliente c)
            {
                // Mensaje de confirmación con advertencia
                string mensaje = $"¿Está seguro de que desea dar de baja a {c.NombreCompleto}?\n\n" +
                                 "IMPORTANTE: Esta acción también dará de baja automáticamente " +
                                 "a todas las mascotas asociadas a este cliente.";

                var resultado = MessageBox.Show(mensaje, "Confirmar Baja de Cliente",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (cs.Desactivar(c.IdCliente))
                        {
                            CargarDatos();

                            MessageBox.Show("Cliente y mascotas desactivados correctamente.", "Éxito",
                                           MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al procesar la baja: {ex.Message}", "Error",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Reactiva un cliente previamente desactivado.
        /// </summary>
        private void BtnReactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button boton && boton.DataContext is Cliente cliente)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar a {cliente.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (cs.Reactivar(cliente.IdCliente))
                        {
                            MessageBox.Show("Cliente reactivado correctamente.", "Información",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo reactivar el cliente.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar cliente: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre la ficha del cliente desde un hyperlink.
        /// </summary>
        private void BtnVerFichaCliente_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Cliente cliente)
                AbrirFicha(cliente.IdCliente);
        }

        /// <summary>
        /// Abre la ficha del cliente desde un botón.
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button boton && boton.DataContext is Cliente cliente)
                AbrirFicha(cliente.IdCliente);
        }

        /// <summary>
        /// Abre la ficha del cliente al hacer doble clic en la tabla.
        /// </summary>
        private void DgClientes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente cliente)
                AbrirFicha(cliente.IdCliente);
        }

        /// <summary>
        /// Abre la ventana de ficha del cliente.
        /// </summary>
        /// <param name="id">ID del cliente a mostrar.</param>
        private void AbrirFicha(int id)
        {
            WindowFichaCliente ventana = new(id) { 
                Owner = Window.GetWindow(this) 
            };

            ventana.ShowDialog();
            CargarDatos();
        }
    }
}