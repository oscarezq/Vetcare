using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Veterinarios
{
    public partial class WindowSelectorVeterinario : Window
    {
        private List<Veterinario> listaVeterinarios;
        public Veterinario VeterinarioSeleccionado { get; set; }
        private VeterinarioService veteService = new VeterinarioService();

        public WindowSelectorVeterinario()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            try
            {
                // Importante: Este método debe devolver el JOIN de Veterinarios con Usuarios
                listaVeterinarios = veteService.ObtenerTodos();
                dgVeterinarios.ItemsSource = listaVeterinarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar veterinarios: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtBuscaVeterinario_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaVeterinarios == null) return;

            string busqueda = txtBuscaVeterinario.Text.ToLower().Trim();

            // Filtrado multizona: Nombre, Apellidos, Especialidad y NumColegiado
            var filtrados = listaVeterinarios.Where(v =>
                (v.Nombre != null && v.Nombre.ToLower().Contains(busqueda)) ||
                (v.Apellidos != null && v.Apellidos.ToLower().Contains(busqueda)) ||
                (v.Especialidad != null && v.Especialidad.ToLower().Contains(busqueda)) ||
                (v.NumeroColegiado != null && v.NumeroColegiado.ToLower().Contains(busqueda))
            ).ToList();

            dgVeterinarios.ItemsSource = filtrados;
        }

        private void FinalizarSeleccion()
        {
            if (dgVeterinarios.SelectedItem is Veterinario vete)
            {
                VeterinarioSeleccionado = vete;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un veterinario de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void dgVeterinarios_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}