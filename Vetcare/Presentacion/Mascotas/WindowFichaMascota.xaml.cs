using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.HistorialesClinicos;

namespace Vetcare.Presentacion.Mascotas
{
    public partial class WindowFichaMascota : Window
    {
        private MascotaService mascotaService = new MascotaService();
        private Mascota mascotaActual;

        public WindowFichaMascota(int idMascota)
        {
            InitializeComponent();
            CargarDatosMascota(idMascota);

            // Cargar por defecto la vista de datos
            MostrarDatos();
        }

        private void CargarDatosMascota(int id)
        {
            try
            {
                mascotaActual = mascotaService.ObtenerPorId(id);
                if (mascotaActual != null)
                {
                    this.DataContext = mascotaActual;
                }
                else
                {
                    MessageBox.Show("No se encontró la mascota.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar: " + ex.Message);
            }
        }

        private void btnDatos_Click(object sender, RoutedEventArgs e) => MostrarDatos();

        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new UC_HistorialClinico(mascotaActual.IdMascota);
        }

        private void MostrarDatos()
        {
            var uc = new UC_DatosMascota(mascotaActual);

            // Ocultar botón de editar si la mascota está inactiva
            if (!mascotaActual.Activo)
                uc.btnEditarInformacion.Visibility = Visibility.Collapsed;

            ContenedorPrincipal.Content = uc;
        }
    }
}