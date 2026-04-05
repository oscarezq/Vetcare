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
    public partial class PageClientes : Page
    {
        private List<Cliente> listaCompleta = new List<Cliente>();
        private readonly ClienteService cs = new ClienteService();

        public PageClientes()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                listaCompleta = cs.ObtenerTodos() ?? new List<Cliente>();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}");
            }
        }

        private void ActualizarTabla()
        {
            // 1. Verificaciones de seguridad
            if (dgClientes == null || rbAsc == null || cbBuscaEstado == null) return;

            // 2. Iniciamos el filtrado sobre la lista completa
            var filtrado = listaCompleta.AsEnumerable();

            // --- Filtros de Texto ---
            if (!string.IsNullOrWhiteSpace(txtBuscaNumDocumento.Text))
                filtrado = filtrado.Where(c => c.NumDocumento.ToLower().Contains(txtBuscaNumDocumento.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
                filtrado = filtrado.Where(c => c.NombreCompleto.ToLower().Contains(txtBuscaCliente.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaTelefono.Text))
                filtrado = filtrado.Where(c => c.Telefono.ToLower().Contains(txtBuscaTelefono.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaEmail.Text))
                filtrado = filtrado.Where(c => c.Email.ToLower().Contains(txtBuscaEmail.Text.ToLower()));

            // --- Filtros de Fecha ---
            if (dtpBuscaFechaDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date >= dtpBuscaFechaDesde.SelectedDate.Value.Date);

            if (dtpBuscaFechaHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date <= dtpBuscaFechaHasta.SelectedDate.Value.Date);

            // --- Filtro de ESTADO (Corregido) ---
            if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado)
            {
                string estadoBusca = itemEstado.Content.ToString();
                if (estadoBusca == "Activo")
                {
                    filtrado = filtrado.Where(c => c.Activo == true);
                }
                else if (estadoBusca == "Inactivo")
                {
                    filtrado = filtrado.Where(c => c.Activo == false);
                }
            }

            // 3. Convertimos a lista para ordenar
            List<Cliente> listaFinal = filtrado.ToList();

            // 4. Ordenación
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool asc = rbAsc.IsChecked == true;

            switch (criterio)
            {
                case "DNI":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.NumDocumento).ToList() : listaFinal.OrderByDescending(x => x.NumDocumento).ToList();
                    break;
                case "Nombre":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.Nombre).ToList() : listaFinal.OrderByDescending(x => x.Nombre).ToList();
                    break;
                case "Apellidos":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.Apellidos).ToList() : listaFinal.OrderByDescending(x => x.Apellidos).ToList();
                    break;
                case "Teléfono":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.Telefono).ToList() : listaFinal.OrderByDescending(x => x.Telefono).ToList();
                    break;
                case "Email":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.Email).ToList() : listaFinal.OrderByDescending(x => x.Email).ToList();
                    break;
                case "Fecha de Alta":
                    listaFinal = asc ? listaFinal.OrderBy(x => x.FechaAlta).ToList() : listaFinal.OrderByDescending(x => x.FechaAlta).ToList();
                    break;
            }

            // 5. Asignar al DataGrid
            dgClientes.ItemsSource = listaFinal;
        }

        private void FiltroAvanzado_Changed(object sender, TextChangedEventArgs e) => ActualizarTabla();
        private void dtpBuscaFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void cbOrdenarPor_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void OrdenDirection_Checked(object sender, RoutedEventArgs e) => ActualizarTabla();
        private void cbBuscaEstado_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
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

        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente win = new WindowCliente { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true) CargarDatos();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Cliente c)
            {
                WindowCliente win = new WindowCliente(c) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true) CargarDatos();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.DataContext is Cliente c)
            {
                // Construimos un mensaje más informativo y visual
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

        private void btnVerFichaCliente_Click(object sender, RoutedEventArgs e)
        {
            Button botonPulsado = sender as Button;
            Cliente clienteDeLaFila = botonPulsado.DataContext as Cliente;

            if (sender is Hyperlink hl && hl.DataContext is Cliente c) AbrirFicha(c.IdCliente);
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            Button botonPulsado = sender as Button;
            Cliente clienteDeLaFila = botonPulsado.DataContext as Cliente;

            if (clienteDeLaFila != null)
            {
                AbrirFicha(clienteDeLaFila.IdCliente);
            }
        }

        private void dgClientes_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente c) AbrirFicha(c.IdCliente);
        }

        private void AbrirFicha(int id)
        {
            WindowFichaCliente win = new WindowFichaCliente(id) { Owner = Window.GetWindow(this) };
            win.ShowDialog();
            CargarDatos();
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button botonPulsado = sender as Button;
                Cliente clienteDeLaFila = botonPulsado.DataContext as Cliente;

                if (clienteDeLaFila != null)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar a {clienteDeLaFila.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (cs.Reactivar(clienteDeLaFila.IdCliente))
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
                MessageBox.Show($"Error al reactivar cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}