using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using System.Linq;

namespace Vetcare.Presentacion.Mascotas.Razas
{
    public partial class WindowRaza : Window
    {
        public WindowRaza()
        {
            InitializeComponent();
            CargarEspecies();
        }

        private void CargarEspecies()
        {
            var lista = new EspecieService().ObtenerTodas();
            cmbEspecies.ItemsSource = lista;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones
            if (cmbEspecies.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar una especie.", "Aviso");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombreRaza.Text))
            {
                MessageBox.Show("Escriba el nombre de la raza.", "Aviso");
                return;
            }

            // Crear objeto
            Raza nueva = new Raza
            {
                NombreRaza = txtNombreRaza.Text.Trim(),
                IdEspecie = (int)cmbEspecies.SelectedValue
            };

            // Guardar en Base de Datos
            bool ok = new RazaService().Insertar(nueva);

            if (ok)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo guardar la raza.");
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}