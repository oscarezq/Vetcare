using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Mascotas.Razas
{
    /// <summary>
    /// Ventana para crear una nueva raza asociada a una especie.
    /// Permite introducir el nombre y guardarlo en base de datos.
    /// </summary>
    public partial class WindowRaza : Window
    {
        // Id de la especie a la que pertenece la raza
        private readonly int _idEspecie;

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        public WindowRaza(int idEspecie)
        {
            InitializeComponent();

            // Guardar especie seleccionada
            this._idEspecie = idEspecie;
        }

        /// <summary>
        /// Guardar nueva raza en base de datos
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validación de campo vacío
            if (string.IsNullOrWhiteSpace(txtNombreRaza.Text))
            {
                MessageBox.Show("Escriba el nombre de la raza.", "Aviso");
                return;
            }

            // Crear objeto raza
            Raza nueva = new()
            {
                NombreRaza = txtNombreRaza.Text.Trim(),
                IdEspecie = _idEspecie
            };

            // Insertar en base de datos
            bool ok = new RazaService().Insertar(nueva);

            // Si se guarda correctamente
            if (ok)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Error al guardar
                MessageBox.Show("No se pudo guardar la raza.");
            }
        }

        /// <summary>
        /// Cerrar ventana sin guardar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}