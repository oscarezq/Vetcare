using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Mascotas.Especies
{
    public partial class WindowNuevaEspecie : Window
    {
        private EspecieService especieService = new EspecieService();
        public Especie EspecieCreada { get; private set; }

        public WindowNuevaEspecie()
        {
            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre de la especie es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Especie nueva = new Especie { NombreEspecie = nombre };
            bool resultado = especieService.Insertar(nueva);

            if (resultado)
            {
                EspecieCreada = nueva;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo guardar la especie.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}