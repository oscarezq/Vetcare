using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Mascotas.Especies
{
    public partial class WindowSelectorEspecie : Window
    {
        private List<Especie> listaEspecies;
        public Especie EspecieSeleccionada { get; set; }

        public WindowSelectorEspecie()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            // Reemplaza con tu método real de servicio
            listaEspecies = new EspecieService().ObtenerTodas();
            dgEspecies.ItemsSource = listaEspecies;
        }

        private void txtBuscaEspecie_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaEspecies == null) return;
            string busqueda = txtBuscaEspecie.Text.ToLower();
            dgEspecies.ItemsSource = listaEspecies.Where(x => x.NombreEspecie.ToLower().Contains(busqueda)).ToList();
        }

        private void FinalizarSeleccion()
        {
            if (dgEspecies.SelectedItem is Especie seleccion)
            {
                EspecieSeleccionada = seleccion;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Seleccione una especie", "Aviso");
            }
        }

        private void dgEspecies_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => FinalizarSeleccion();
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btnNuevaEspecie_Click(object sender, RoutedEventArgs e)
        {
            // Abrimos la ventana para crear nueva especie
            WindowNuevaEspecie ventana = new WindowNuevaEspecie();
            ventana.Owner = this;

            if (ventana.ShowDialog() == true)
            {
                // Recargamos el DataGrid con la especie nueva
                CargarLista(); // Método que lista las especies desde la BD
                dgEspecies.SelectedItem = ventana.EspecieCreada; // Seleccionamos la creada
                dgEspecies.ScrollIntoView(ventana.EspecieCreada);
            }
        }
    }
}