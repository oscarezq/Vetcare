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
        private int _idEspecieFiltrar;

        public Raza RazaSeleccionada { get; set; }

        public WindowSelectorRaza(int idEspecie = 0)
        {
            InitializeComponent();
            _idEspecieFiltrar = idEspecie;
            CargarLista();
        }

        private void CargarLista()
        {
            if (_idEspecieFiltrar > 0)
            {
                listaRazas = new RazaService().ObtenerPorEspecie(_idEspecieFiltrar);
            }
            else
            {
                listaRazas = new RazaService().ObtenerTodas();
            }

            dgRazas.ItemsSource = listaRazas;
        }

        private void txtBuscaRaza_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaRazas == null) return;

            string busqueda = txtBuscaRaza.Text.ToLower();
            dgRazas.ItemsSource = listaRazas
                .Where(r => r.NombreRaza.ToLower().Contains(busqueda))
                .ToList();
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

        private void dgRazas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FinalizarSeleccion();
        }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            FinalizarSeleccion();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnNuevaRaza_Click(object sender, RoutedEventArgs e)
        {
            WindowRaza ventana = new WindowRaza(_idEspecieFiltrar);
            ventana.Owner = this;

            if (ventana.ShowDialog() == true)
            {
                CargarLista();
                MessageBox.Show("Raza guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}