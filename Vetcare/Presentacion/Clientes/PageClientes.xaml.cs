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
            if (dgClientes == null || rbAsc == null) return;

            var filtrado = listaCompleta.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtBuscaNumDocumento.Text))
                filtrado = filtrado.Where(c => c.NumDocumento.ToLower().Contains(txtBuscaNumDocumento.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
                filtrado = filtrado.Where(c => c.NombreCompleto.ToLower().Contains(txtBuscaCliente.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaTelefono.Text))
                filtrado = filtrado.Where(c => c.Telefono.ToLower().Contains(txtBuscaTelefono.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaEmail.Text))
                filtrado = filtrado.Where(c => c.Email.ToLower().Contains(txtBuscaEmail.Text.ToLower()));

            if (dtpBuscaFechaDesde.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date >= dtpBuscaFechaDesde.SelectedDate.Value.Date);

            if (dtpBuscaFechaHasta.SelectedDate.HasValue)
                filtrado = filtrado.Where(c => c.FechaAlta.Date <= dtpBuscaFechaHasta.SelectedDate.Value.Date);

            List<Cliente> listaFinal = filtrado.ToList();
            string criterio = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool asc = rbAsc.IsChecked == true;

            switch (criterio)
            {
                case "DNI": listaFinal = asc ? listaFinal.OrderBy(x => x.NumDocumento).ToList() : listaFinal.OrderByDescending(x => x.NumDocumento).ToList(); break;
                case "Nombre": listaFinal = asc ? listaFinal.OrderBy(x => x.Nombre).ToList() : listaFinal.OrderByDescending(x => x.Nombre).ToList(); break;
                case "Apellidos": listaFinal = asc ? listaFinal.OrderBy(x => x.Apellidos).ToList() : listaFinal.OrderByDescending(x => x.Apellidos).ToList(); break;
                case "Teléfono": listaFinal = asc ? listaFinal.OrderBy(x => x.Telefono).ToList() : listaFinal.OrderByDescending(x => x.Telefono).ToList(); break;
                case "Email": listaFinal = asc ? listaFinal.OrderBy(x => x.Email).ToList() : listaFinal.OrderByDescending(x => x.Email).ToList(); break;
                case "Fecha de Alta": listaFinal = asc ? listaFinal.OrderBy(x => x.FechaAlta).ToList() : listaFinal.OrderByDescending(x => x.FechaAlta).ToList(); break;
            }

            dgClientes.ItemsSource = listaFinal;
        }

        private void FiltroAvanzado_Changed(object sender, TextChangedEventArgs e) => ActualizarTabla();
        private void dtpBuscaFecha_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void cbOrdenarPor_SelectionChanged(object sender, SelectionChangedEventArgs e) => ActualizarTabla();
        private void OrdenDirection_Checked(object sender, RoutedEventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaNumDocumento.Clear();
            txtBuscaCliente.Clear();
            txtBuscaTelefono.Clear();
            txtBuscaEmail.Clear();
            dtpBuscaFechaDesde.SelectedDate = null;
            dtpBuscaFechaHasta.SelectedDate = null;
            cbOrdenarPor.SelectedIndex = 0;
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
                if (MessageBox.Show($"¿Eliminar a {c.NombreCompleto}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (cs.Eliminar(c.IdCliente)) CargarDatos();
                }
            }
        }

        private void btnEliminarVarios_Click(object sender, RoutedEventArgs e)
        {
            var seleccion = dgClientes.SelectedItems.Cast<Cliente>().ToList();
            if (MessageBox.Show($"¿Eliminar {seleccion.Count} clientes?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (cs.EliminarVarias(seleccion.Select(x => x.IdCliente).ToList())) CargarDatos();
            }
        }

        private void btnVerFichaCliente_Click(object sender, RoutedEventArgs e)
        {
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

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool esAdmin = Sesion.UsuarioActual?.IdRol != 2;
            btnEliminarVarios.Visibility = (esAdmin && dgClientes.SelectedItems.Count > 1) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}