using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            try
            {
                if (listaCompleta == null || rbAsc == null || dgUsuarios == null) return;

                // FILTRADO
                List<Usuario> listaFiltrada = new List<Usuario>();

                foreach (Usuario u in listaCompleta)
                {
                    string busquedaUser = txtBuscaUsername.Text.ToLower();
                    string busquedaNombre = txtBuscaNombre.Text.ToLower();

                    // Filtro Username
                    if (!string.IsNullOrEmpty(busquedaUser) && !u.Username.ToLower().Contains(busquedaUser)) continue;

                    // Filtro Nombre/Apellidos
                    string nombreCompleto = (u.Nombre + " " + u.Apellidos).ToLower();
                    if (!string.IsNullOrEmpty(busquedaNombre) && !nombreCompleto.Contains(busquedaNombre)) continue;

                    // Filtro Rol
                    if (cbBuscaRol.SelectedItem is ComboBoxItem itemRol && itemRol.Content.ToString() != "Todos")
                    {
                        if (u.NombreRol != itemRol.Content.ToString()) continue;
                    }

                    // Filtro Estado
                    if (cbBuscaEstado.SelectedItem is ComboBoxItem itemEstado && itemEstado.Content.ToString() != "Todos")
                    {
                        bool buscarActivo = itemEstado.Content.ToString() == "Activo";
                        if (u.Activo != buscarActivo) continue;
                    }

                    listaFiltrada.Add(u);
                }

                // ORDENACIÓN
                if (cbOrdenarPor.SelectedItem is ComboBoxItem itemOrden)
                {
                    string criterio = itemOrden.Content.ToString();
                    bool esAscendente = (bool)rbAsc.IsChecked;

                    switch (criterio)
                    {
                        case "Username":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Username.CompareTo(y.Username) : y.Username.CompareTo(x.Username));
                            break;
                        case "Nombre":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.Nombre.CompareTo(y.Nombre) : y.Nombre.CompareTo(x.Nombre));
                            break;
                        case "Rol":
                            listaFiltrada.Sort((x, y) => esAscendente ? x.NombreRol.CompareTo(y.NombreRol) : y.NombreRol.CompareTo(x.NombreRol));
                            break;
                    }
                }

                dgUsuarios.ItemsSource = listaFiltrada;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar: {ex.Message}");
            }
        }

        private void FiltroUsuario_Changed(object sender, EventArgs e) => ActualizarTabla();

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaUsername.Clear();
            txtBuscaNombre.Clear();
            cbBuscaRol.SelectedIndex = 0;
            cbBuscaEstado.SelectedIndex = 0;
            cbOrdenarPor.SelectedIndex = 0;
            rbAsc.IsChecked = true;
            ActualizarTabla();
        }

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
    }
}