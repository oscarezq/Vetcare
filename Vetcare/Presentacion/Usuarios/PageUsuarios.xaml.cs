using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Usuarios
{
    public partial class PageUsuarios : Page
    {
        private List<Usuario> listaCompleta = new List<Usuario>();
        private UsuarioService us = new UsuarioService();

        public PageUsuarios()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                listaCompleta = us.ObtenerTodos() ?? new List<Usuario>();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarTabla()
        {
            if (dgUsuarios == null || rbAsc == null) return;

            var filtrados = listaCompleta.AsEnumerable();

            // Filtros de texto
            if (!string.IsNullOrWhiteSpace(txtBuscaUsername.Text))
                filtrados = filtrados.Where(u => u.Username.ToLower().Contains(txtBuscaUsername.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
                filtrados = filtrados.Where(u => u.Nombre.ToLower().Contains(txtBuscaNombre.Text.ToLower()));

            if (!string.IsNullOrWhiteSpace(txtBuscaEmail.Text))
                filtrados = filtrados.Where(u => u.Email.ToLower().Contains(txtBuscaEmail.Text.ToLower()));

            // Filtro por Rol
            if (cbBuscaRol.SelectedItem is ComboBoxItem itemRol && !string.IsNullOrEmpty(itemRol.Content.ToString()))
                filtrados = filtrados.Where(u => u.NombreRol == itemRol.Content.ToString());

            // Filtro por Estado
            if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado && !string.IsNullOrEmpty(itemEstado.Content.ToString()))
            {
                bool buscarActivo = itemEstado.Content.ToString() == "Activo";
                filtrados = filtrados.Where(u => u.Activo == buscarActivo);
            }

            // Filtro por Fechas
            if (dpBuscaFechaDesde.SelectedDate.HasValue)
                filtrados = filtrados.Where(u => u.FechaAlta.Date >= dpBuscaFechaDesde.SelectedDate.Value.Date);

            if (dpBuscaFechaHasta.SelectedDate.HasValue)
                filtrados = filtrados.Where(u => u.FechaAlta.Date <= dpBuscaFechaHasta.SelectedDate.Value.Date);

            // Ordenación
            List<Usuario> listaFinal = filtrados.ToList();
            string campoOrden = (cbOrdenarPor.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool esAsc = rbAsc.IsChecked == true;

            switch (campoOrden)
            {
                case "Username": listaFinal = esAsc ? listaFinal.OrderBy(u => u.Username).ToList() : listaFinal.OrderByDescending(u => u.Username).ToList(); break;
                case "Nombre": listaFinal = esAsc ? listaFinal.OrderBy(u => u.Nombre).ToList() : listaFinal.OrderByDescending(u => u.Nombre).ToList(); break;
                case "Rol": listaFinal = esAsc ? listaFinal.OrderBy(u => u.NombreRol).ToList() : listaFinal.OrderByDescending(u => u.NombreRol).ToList(); break;
                case "Fecha Alta": listaFinal = esAsc ? listaFinal.OrderBy(u => u.FechaAlta).ToList() : listaFinal.OrderByDescending(u => u.FechaAlta).ToList(); break;
            }

            dgUsuarios.ItemsSource = listaFinal;
        }

        private void FiltroUsuario_Changed(object sender, EventArgs e) => ActualizarTabla();
        private void FiltroUsuario_Changed(object sender, SelectionChangedEventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaUsername.Clear();
            txtBuscaNombre.Clear();
            txtBuscaEmail.Clear();
            dpBuscaFechaDesde.SelectedDate = null;
            dpBuscaFechaHasta.SelectedDate = null;
            cbBuscaRol.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            WindowUsuario win = new WindowUsuario { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true) CargarDatos();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                WindowUsuario win = new WindowUsuario(u) { Owner = Window.GetWindow(this) };

                if (win.ShowDialog() == true) CargarDatos();
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                string accion = u.Activo ? "desactivar" : "activar";
                if (MessageBox.Show($"¿Desea {accion} al usuario {u.Username}?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (us.Eliminar(u.IdUsuario)) CargarDatos();
                }
            }
        }

        private void HyperlinkUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Usuario u) AbrirFichaUsuario(u.IdUsuario);
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u) AbrirFichaUsuario(u.IdUsuario);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario u) AbrirFichaUsuario(u.IdUsuario);
        }

        private void AbrirFichaUsuario(int idUsuario)
        {
            WindowFichaUsuario ficha = new WindowFichaUsuario(idUsuario) { Owner = Window.GetWindow(this) };
            ficha.ShowDialog();
            CargarDatos();
        }

        private void btnReactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button botonPulsado = sender as Button;
                Usuario usuarioDeLaFila = botonPulsado.DataContext as Usuario;

                if (usuarioDeLaFila != null)
                {
                    MessageBoxResult confirmacion = MessageBox.Show(
                        $"¿Deseas reactivar a {usuarioDeLaFila.Nombre}?",
                        "Confirmar acción",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmacion == MessageBoxResult.Yes)
                    {
                        if (us.Reactivar(usuarioDeLaFila.IdUsuario))
                        {
                            MessageBox.Show("Usuario reactivado correctamente.", "Información",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            CargarDatos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo reactivar el usuario.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}