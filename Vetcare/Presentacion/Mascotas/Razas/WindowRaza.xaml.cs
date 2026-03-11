using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using System.Linq;

namespace Vetcare.Presentacion.Mascotas.Razas
{
    public partial class WindowRaza : Window
    {
        private int _idEspecie;
        private RazaService razaService = new RazaService();

        public WindowRaza(int idEspecie)
        {
            InitializeComponent();
            this._idEspecie = idEspecie;
            CargarEspecies();
        }

        private void CargarEspecies()
        {
            var lista = new EspecieService().ObtenerTodas();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtNombreRaza.Text))
            {
                MessageBox.Show("Escriba el nombre de la raza.", "Aviso");
                return;
            }

            // Crear objeto
            Raza nueva = new Raza
            {
                NombreRaza = txtNombreRaza.Text.Trim(),
                IdEspecie = _idEspecie
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