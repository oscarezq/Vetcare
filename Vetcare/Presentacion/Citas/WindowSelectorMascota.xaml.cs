using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Ventana selector de mascotas para elegir una mascota existente o crear una nueva.
    /// </summary>
    public partial class WindowSelectorMascota : Window
    {
        // Lista local de mascotas cargadas desde la base de datos
        private List<Mascota>? listaMascotas;

        // Mascota seleccionada que se devolverá al cerrar la ventana
        public Mascota? MascotaSeleccionada;

        /// <summary>
        /// Constructor de la ventana selector de mascotas
        /// </summary>
        public WindowSelectorMascota()
        {
            InitializeComponent();

            // Cargar todas las mascotas activas
            CargarLista();
        }

        /// <summary>
        /// Carga la lista de mascotas activas desde el servicio
        /// </summary>
        private void CargarLista()
        {
            listaMascotas = new MascotaService().ObtenerTodas()
                                    .Where(m => m.Activo == true)
                                    .ToList();

            dgMascotas.ItemsSource = listaMascotas;
        }

        /// <summary>
        /// Filtra las mascotas según el texto de búsqueda
        /// </summary>
        private void TxtBuscaMascota_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaMascotas == null) return;

            string busq = txtBuscaMascota.Text.ToLower().Trim();

            dgMascotas.ItemsSource = listaMascotas.Where(m =>
                m.Nombre!.ToLower().Contains(busq) ||
                m.NombreEspecie!.ToLower().Contains(busq) ||
                (m.NombreDueno != null && m.NombreDueno.ToLower().Contains(busq))
            ).ToList();
        }

        /// <summary>
        /// Finaliza la selección de la mascota actual del DataGrid
        /// </summary>
        private void Finalizar()
        {
            if (dgMascotas.SelectedItem is Mascota m)
            {
                MascotaSeleccionada = m;
                this.DialogResult = true;
            }
            else MessageBox.Show("Seleccione una mascota.");
        }

        /// <summary>
        /// Abre la ventana para crear una nueva mascota
        /// </summary>
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana de registro de mascota (modo nuevo)
            WindowMascota ventanaMascota = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventanaMascota.ShowDialog() == true)
            {
                // Refrescar lista después de crear mascota
                CargarLista();

                // Seleccionar automáticamente la última creada
                if (listaMascotas != null && listaMascotas.Count > 0)
                {
                    var nueva = listaMascotas.OrderByDescending(m => m.IdMascota).FirstOrDefault();
                    dgMascotas.SelectedItem = nueva;
                    dgMascotas.ScrollIntoView(nueva);
                }
            }
        }

        // Seleccionar mascota con botón
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e) => Finalizar();

        // Seleccionar mascota con doble clic en el DataGrid
        private void DgMascotas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => Finalizar();

        // Cerrar ventana sin seleccionar
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}