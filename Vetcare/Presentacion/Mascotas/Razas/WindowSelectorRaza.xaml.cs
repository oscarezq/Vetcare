using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Mascotas.Razas
{
    public partial class WindowSelectorRaza : Window
    {
        private List<Raza> listaRazas;
        public Raza RazaSeleccionada { get; set; }
        private int _idEspecieFiltrar;

        public WindowSelectorRaza(int idEspecie = 0)
        {
            InitializeComponent();
            _idEspecieFiltrar = idEspecie;
            CargarLista();
        }

        private void CargarLista()
        {
            var todas = new RazaService().ObtenerTodas();

            // Si se pasó una especie desde la ventana anterior, filtramos
            if (_idEspecieFiltrar > 0)
            {
                listaRazas = todas.Where(r => r.IdEspecie == _idEspecieFiltrar).ToList();
            }
            else
            {
                listaRazas = todas;
            }

            dgRazas.ItemsSource = listaRazas;
        }

        private void txtBuscaRaza_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaRazas == null) return;
            string busqueda = txtBuscaRaza.Text.ToLower();
            dgRazas.ItemsSource = listaRazas.Where(x => x.NombreRaza.ToLower().Contains(busqueda)).ToList();
        }

        private void FinalizarSeleccion()
        {
            if (dgRazas.SelectedItem is Raza seleccion)
            {
                RazaSeleccionada = seleccion;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Seleccione una raza", "Aviso");
            }
        }

        private void dgRazas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => FinalizarSeleccion();
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btnNuevaRaza_Click(object sender, RoutedEventArgs e)
        {
            // Pasamos el ID de la especie actual si queremos que la nueva raza se asigne a ella automáticamente
            WindowRaza ventana = new WindowRaza();
            ventana.Owner = this;

            if (ventana.ShowDialog() == true)
            {
                // Recargamos la lista desde la base de datos para ver la nueva raza
                CargarLista();
                MessageBox.Show("Raza guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}