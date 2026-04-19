using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Mascotas.Especies
{
    /// <summary>
    /// Ventana para crear una nueva especie de mascota.
    /// Permite introducir el nombre y guardarlo en la base de datos.
    /// </summary>
    public partial class WindowNuevaEspecie : Window
    {
        // Servicio encargado de la lógica de especies
        private readonly EspecieService especieService = new();

        // Propiedad que devuelve la especie creada tras el guardado
        public Especie? EspecieCreada;

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        public WindowNuevaEspecie()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento del botón Guardar
        /// Valida el nombre e inserta la especie en la base de datos
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener nombre introducido por el usuario
            string nombre = txtNombre.Text.Trim();

            // Validación de campo obligatorio
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre de la especie es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Insertar en base de datos
            Especie nueva = new() { NombreEspecie = nombre };
            bool resultado = especieService.Insertar(nueva);

            // Si se inserta correctamente
            if (resultado)
            {
                // Guardar especie creada para devolverla a la ventana anterior
                EspecieCreada = nueva;

                // Indicar resultado positivo y cerrar ventana
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Mostrar error si falla la inserción
                MessageBox.Show("No se pudo guardar la especie.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Evento del botón Cancelar
        /// Cierra la ventana sin guardar cambios
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}