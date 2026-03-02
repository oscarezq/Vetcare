using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                listaCompleta = us.ObtenerTodos();
                ActualizarTabla();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ActualizarTabla()
        {
            if (cbBuscaEstado == null || dpBuscaFechaDesde == null || dpBuscaFechaHasta == null || rbAsc == null || dgUsuarios == null)
                return;

            // CORRECCIÓN: Usar 'listaCompleta' que es la que declaraste arriba
            if (listaCompleta == null) return;

            // Empezamos con la colección cargada desde la DB
            var filtrados = listaCompleta.AsEnumerable();

            // --- FILTROS DE TEXTO ---
            if (!string.IsNullOrWhiteSpace(txtBuscaUsername.Text))
                filtrados = filtrados.Where(u => u.Username?.ToLower().Contains(txtBuscaUsername.Text.ToLower()) ?? false);

            if (!string.IsNullOrWhiteSpace(txtBuscaNombre.Text))
                filtrados = filtrados.Where(u => u.Nombre?.ToLower().Contains(txtBuscaNombre.Text.ToLower()) ?? false);

            if (!string.IsNullOrWhiteSpace(txtBuscaApellidos.Text))
                filtrados = filtrados.Where(u => u.Apellidos?.ToLower().Contains(txtBuscaApellidos.Text.ToLower()) ?? false);

            if (!string.IsNullOrWhiteSpace(txtBuscaEmail.Text))
                filtrados = filtrados.Where(u => u.Email?.ToLower().Contains(txtBuscaEmail.Text.ToLower()) ?? false);

            if (!string.IsNullOrWhiteSpace(txtBuscaTelefono.Text))
                filtrados = filtrados.Where(u => u.Telefono?.Contains(txtBuscaTelefono.Text) ?? false);

            // --- FILTRO POR ROL ---
            if (cbBuscaRol.SelectedItem is ComboBoxItem itemRol && !string.IsNullOrEmpty(itemRol.Content.ToString()))
            {
                string rolBusqueda = itemRol.Content.ToString();
                filtrados = filtrados.Where(u => u.NombreRol == rolBusqueda);
            }

            // --- FILTRO POR ESTADO (Corregido para comparar con el texto del ComboBox) ---
            if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado)
            {
                string estadoSeleccionado = itemEstado.Content.ToString();
                if (estadoSeleccionado == "Activo")
                    filtrados = filtrados.Where(u => u.Activo == true);
                else if (estadoSeleccionado == "Inactivo")
                    filtrados = filtrados.Where(u => u.Activo == false);
            }

            // --- FILTRO POR FECHAS ---
            if (dpBuscaFechaDesde.SelectedDate.HasValue)
                filtrados = filtrados.Where(u => u.FechaAlta.Date >= dpBuscaFechaDesde.SelectedDate.Value.Date);

            if (dpBuscaFechaHasta.SelectedDate.HasValue)
                filtrados = filtrados.Where(u => u.FechaAlta.Date <= dpBuscaFechaHasta.SelectedDate.Value.Date);

            // --- ORDENACIÓN ---
            if (cbOrdenarPor.SelectedItem is ComboBoxItem itemOrden)
            {
                string campo = itemOrden.Content.ToString();
                bool esAsc = rbAsc.IsChecked == true;
                switch (campo)
                {
                    case "Username": filtrados = esAsc ? filtrados.OrderBy(u => u.Username) : filtrados.OrderByDescending(u => u.Username); break;
                    case "Nombre": filtrados = esAsc ? filtrados.OrderBy(u => u.Nombre) : filtrados.OrderByDescending(u => u.Nombre); break;
                    case "Rol": filtrados = esAsc ? filtrados.OrderBy(u => u.NombreRol) : filtrados.OrderByDescending(u => u.NombreRol); break;
                    case "Fecha Alta": filtrados = esAsc ? filtrados.OrderBy(u => u.FechaAlta) : filtrados.OrderByDescending(u => u.FechaAlta); break;
                }
            }

            // 3. Asignar al DataGrid
            dgUsuarios.ItemsSource = filtrados.ToList();
        }

        // Actualiza el botón limpiar para resetear los nuevos campos
        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaUsername.Clear();
            txtBuscaNombre.Clear();
            txtBuscaApellidos.Clear();
            txtBuscaEmail.Clear();
            txtBuscaTelefono.Clear();
            dpBuscaFechaDesde.SelectedDate = null;
            dpBuscaFechaHasta.SelectedDate = null;
            cbBuscaRol.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

        private void FiltroUsuario_Changed(object sender, EventArgs e) => ActualizarTabla();

        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            WindowUsuario win = new WindowUsuario();
            win.ShowDialog();
            CargarDatos();
        }

        private void btnEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                var result = MessageBox.Show($"¿Desactivar al usuario {u.Username}?", "Confirmar", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (us.Eliminar(u.IdUsuario)) CargarDatos();
                }
            }
        }

        private void btnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // 1. Obtenemos el usuario seleccionado a través del DataContext del botón
            if (sender is Button btn && btn.DataContext is Usuario u)
            {
                // 2. Opcional pero recomendado: Crear una copia para evitar cambios en UI si se cancela
                // Si no tienes un método Clone, simplemente asegúrate de llamar a CargarDatos() después.

                // 3. Instanciamos la ventana pasando el usuario seleccionado
                WindowUsuario win = new WindowUsuario(u);

                // 4. Mostramos como diálogo
                if (win.ShowDialog() == true)
                {
                    // 5. Si la ventana devolvió DialogResult = true, refrescamos la lista
                    CargarDatos();
                }
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Obtenemos la fila seleccionada
            var grid = sender as DataGrid;
            if (grid != null && grid.SelectedItem != null)
            {
                // Casteamos al objeto Usuario (ajusta el nombre de tu clase si es distinto)
                var usuarioSeleccionado = grid.SelectedItem as Usuario;

                if (usuarioSeleccionado != null)
                {
                    AbrirFichaUsuario(usuarioSeleccionado.IdUsuario);
                }
            }
        }

        // Método auxiliar para no repetir código
        private void AbrirFichaUsuario(int idUsuario)
        {
            WindowFichaUsuario ficha = new WindowFichaUsuario(idUsuario);
            ficha.Owner = Window.GetWindow(this);
            ficha.ShowDialog();
            CargarDatos();
        }

        private void HyperlinkUsuario_Click(object sender, RoutedEventArgs e)
        {
            // 1. El remitente (sender) es el Hyperlink que recibió el clic
            if (sender is System.Windows.Documents.Hyperlink hl)
            {
                // 2. El DataContext del Hyperlink en un DataGrid es el objeto de la fila (Usuario)
                if (hl.DataContext is Usuario u)
                {
                    // 3. Reutilizamos tu método para abrir la ficha
                    AbrirFichaUsuario(u.IdUsuario);
                }
            }
        }
    }
}