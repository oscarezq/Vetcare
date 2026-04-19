using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Mascotas.Especies
{
    /// <summary>
    /// Ventana para seleccionar una especie existente o crear una nueva.
    /// Permite búsqueda, selección y alta de especies.
    /// </summary>
    public partial class WindowSelectorEspecie : Window
    {
        // Lista completa de especies cargadas desde base de datos
        private List<Especie>? listaEspecies;

        // Especie seleccionada por el usuario
        public Especie? EspecieSeleccionada;

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        public WindowSelectorEspecie()
        {
            InitializeComponent();

            // Cargar lista inicial de especies
            CargarLista();
        }

        /// <summary>
        /// Carga todas las especies desde la base de datos
        /// </summary>
        private void CargarLista()
        {
            listaEspecies = new EspecieService().ObtenerTodas();
            dgEspecies.ItemsSource = listaEspecies;
        }

        /// <summary>
        /// Filtra especies según el texto introducido en la búsqueda
        /// </summary>
        private void TxtBuscaEspecie_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaEspecies == null) return;

            string busqueda = txtBuscaEspecie.Text.ToLower();

            dgEspecies.ItemsSource = listaEspecies
                .Where(x => x.NombreEspecie!.ToLower().Contains(busqueda))
                .ToList();
        }

        /// <summary>
        /// Finaliza la selección de una especie
        /// </summary>
        private void FinalizarSeleccion()
        {
            // Comprobar que hay elemento seleccionado
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

        /// <summary>
        /// Selección con doble clic en el DataGrid
        /// </summary>
        private void DgEspecies_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => FinalizarSeleccion();

        /// <summary>
        /// Botón seleccionar especie
        /// </summary>
        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
            => FinalizarSeleccion();

        /// <summary>
        /// Cierra la ventana sin seleccionar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
            => this.Close();

        /// <summary>
        /// Abre ventana para crear nueva especie y refresca lista
        /// </summary>
        private void BtnNuevaEspecie_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de nueva especie
            WindowNuevaEspecie ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Si se guarda correctamente, recargar lista
            if (ventana.ShowDialog() == true)
            {
                CargarLista();

                MessageBox.Show("Especie guardada correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Seleccionar automáticamente la nueva especie
                dgEspecies.SelectedItem = ventana.EspecieCreada;
                dgEspecies.ScrollIntoView(ventana.EspecieCreada);
            }
        }
    }
}