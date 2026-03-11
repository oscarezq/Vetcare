using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion
{
    public partial class WindowSelectorMascota : Window
    {
        private List<Mascota> listaMascotas;
        public Mascota MascotaSeleccionada { get; set; }

        public WindowSelectorMascota()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            listaMascotas = new MascotaService().ObtenerTodas();
            dgMascotas.ItemsSource = listaMascotas;
        }

        private void txtBuscaMascota_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaMascotas == null) return;
            string busq = txtBuscaMascota.Text.ToLower().Trim();
            dgMascotas.ItemsSource = listaMascotas.Where(m =>
                m.Nombre.ToLower().Contains(busq) ||
                m.NombreEspecie.ToLower().Contains(busq) ||
                (m.NombreDueno != null && m.NombreDueno.ToLower().Contains(busq))
            ).ToList();
        }

        private void Finalizar()
        {
            if (dgMascotas.SelectedItem is Mascota m)
            {
                MascotaSeleccionada = m;
                this.DialogResult = true;
            }
            else MessageBox.Show("Seleccione una mascota.");
        }

        private void btnNuevo_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana de registro de mascota (sin pasarle objeto para que sea modo "Nuevo")
            WindowMascota ventanaMascota = new WindowMascota();
            ventanaMascota.Owner = this;

            if (ventanaMascota.ShowDialog() == true)
            {
                // Refrescamos la lista para que aparezca la que acabas de crear
                CargarLista();

                // Opcional: Si quieres que se seleccione automáticamente la nueva
                if (listaMascotas != null && listaMascotas.Count > 0)
                {
                    // Buscamos la mascota con el ID más alto (la última creada)
                    var nueva = listaMascotas.OrderByDescending(m => m.IdMascota).FirstOrDefault();
                    dgMascotas.SelectedItem = nueva;
                    dgMascotas.ScrollIntoView(nueva);
                }
            }
        }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e) => Finalizar();
        private void dgMascotas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => Finalizar();
        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}