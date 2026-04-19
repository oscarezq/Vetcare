using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Mascotas.Razas
{
    /// <summary>
    /// Ventana para seleccionar una raza existente filtrada por especie.
    /// Permite búsqueda, selección y creación de nuevas razas.
    /// </summary>
    public partial class WindowSelectorRaza : Window
    {
        // Lista completa de razas cargadas
        private List<Raza>? listaRazas;

        // Id de la especie para filtrar razas
        private readonly int _idEspecieFiltrar;

        // Raza seleccionada por el usuario
        public Raza? RazaSeleccionada;

        /// <summary>
        /// Constructor de la ventana
        /// </summary>
        public WindowSelectorRaza(int idEspecie = 0)
        {
            InitializeComponent();

            // Guardar especie para filtrado
            _idEspecieFiltrar = idEspecie;

            // Cargar datos iniciales
            CargarLista();
        }

        /// <summary>
        /// Carga las razas según la especie seleccionada
        /// </summary>
        private void CargarLista()
        {
            // Si hay especie seleccionada, filtrar por ella
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

        /// <summary>
        /// Filtra razas según texto de búsqueda
        /// </summary>
        private void TxtBuscaRaza_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaRazas == null) return;

            // Texto de búsqueda en minúsculas
            string busqueda = txtBuscaRaza.Text.ToLower().Trim();

            // Filtrar lista actual
            var listaFiltrada = listaRazas
                .Where(r => r.NombreRaza!.ToLower().Contains(busqueda))
                .ToList();

            dgRazas.ItemsSource = listaFiltrada;
        }

        /// <summary>
        /// Finaliza selección de raza
        /// </summary>
        private void FinalizarSeleccion()
        {
            // Validar selección
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

        /// <summary>
        /// Selección con doble clic
        /// </summary>
        private void DgRazas_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => FinalizarSeleccion();

        /// <summary>
        /// Botón seleccionar
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();

        /// <summary>
        /// Botón cancelar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        /// <summary>
        /// Abre ventana para crear nueva raza y recarga lista
        /// </summary>
        private void BtnNuevaRaza_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de nueva raza
            WindowRaza ventana = new(_idEspecieFiltrar)
            {
                Owner = this
            };

            // Si se guarda correctamente, refrescar datos
            if (ventana.ShowDialog() == true)
            {
                CargarLista();
                MessageBox.Show("Raza guardada correctamente.", "Éxito",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}