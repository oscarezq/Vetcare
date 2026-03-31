using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion.Mascotas
{
    public partial class UC_DatosMascota : UserControl
    {
        private MascotaService mascotaService = new MascotaService();
        private Mascota mascotaActual;

        public UC_DatosMascota(Mascota mascota)
        {
            InitializeComponent();
            this.mascotaActual = mascota;
            this.DataContext = mascotaActual;
        }

        private void btnEditarMascota_Click(object sender, RoutedEventArgs e)
        {
            if (Sesion.UsuarioActual.IdRol == 2)
            {
                MessageBox.Show("No tienes permisos para editar esta información.");
                return;
            }

            try
            {
                // Abrir ventana de edición (asumiendo que tienes WindowMascota)
                WindowMascota ventanaEditar = new WindowMascota(mascotaActual);

                // Configuración de edición
                ventanaEditar.Owner = Window.GetWindow(this);
                bool? resultado = ventanaEditar.ShowDialog();

                // Si se guardaron cambios, refrescamos el objeto
                if (resultado == true)
                {
                    mascotaActual = mascotaService.ObtenerPorId(mascotaActual.IdMascota);
                    this.DataContext = mascotaActual;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        private void HyperlinkDueno_Click(object sender, RoutedEventArgs e)
        {
            if (mascotaActual != null && mascotaActual.IdCliente > 0)
            {
                // Abrir ficha del dueño
                WindowFichaCliente ventanaDueno = new WindowFichaCliente(mascotaActual.IdCliente);
                ventanaDueno.Owner = Window.GetWindow(this);
                ventanaDueno.ShowDialog();
            }
        }
    }
}