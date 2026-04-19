using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.HistorialesClinicos;

namespace Vetcare.Presentacion.Mascotas
{
    /// <summary>
    /// Ventana principal de ficha de mascota.
    /// Permite navegar entre datos generales e historial clínico.
    /// </summary>
    public partial class WindowFichaMascota : Window
    {
        // Servicio de negocio para acceder a datos de mascotas
        private readonly MascotaService mascotaService = new();

        // Mascota actualmente cargada en la ficha
        private Mascota? mascotaActual;

        /// <summary>
        /// Constructor de la ficha de mascota.
        /// Recibe el ID de la mascota y carga sus datos.
        /// </summary>
        public WindowFichaMascota(int idMascota)
        {
            InitializeComponent();

            // Cargamos los datos de la mascota
            CargarDatosMascota(idMascota);

            // Vista inicial por defecto: datos generales
            MostrarDatos();
        }

        /// <summary>
        /// Carga la información de la mascota desde la base de datos.
        /// </summary>
        private void CargarDatosMascota(int id)
        {
            try
            {
                // Obtenemos la mascota por su ID
                mascotaActual = mascotaService.ObtenerPorId(id);

                if (mascotaActual != null)
                {
                    // Asignamos DataContext para binding en XAML
                    this.DataContext = mascotaActual;
                }
                else
                {
                    // Si no existe, mostramos error y cerramos ventana
                    MessageBox.Show("No se encontró la mascota.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores de carga
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
        }

        /// <summary>
        /// Botón que muestra la vista de datos generales.
        /// </summary>
        private void BtnDatos_Click(object sender, RoutedEventArgs e) => MostrarDatos();

        /// <summary>
        /// Botón que carga el historial clínico de la mascota.
        /// </summary>
        private void BtnHistorial_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new UC_HistorialClinico(mascotaActual!.IdMascota);
        }

        /// <summary>
        /// Muestra la vista de datos generales de la mascota.
        /// </summary>
        private void MostrarDatos()
        {
            // Creamos el UserControl de datos
            var uc = new UC_DatosMascota(mascotaActual!);

            // Si la mascota está inactiva, ocultamos el botón de edición
            if (!mascotaActual!.Activo)
                uc.btnEditarInformacion.Visibility = Visibility.Collapsed;

            // Mostramos la vista en el contenedor principal
            ContenedorPrincipal.Content = uc;
        }
    }
}