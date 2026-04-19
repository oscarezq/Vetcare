using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion.Mascotas
{
    /// <summary>
    /// Control de usuario que muestra la ficha completa de una mascota.
    /// Se encarga de mostrar datos del paciente y permitir acciones como edición o consulta del dueño.
    /// </summary>
    public partial class UC_DatosMascota : UserControl
    {
        // Servicio de mascotas para obtener datos actualizados desde la base de datos
        private readonly MascotaService mascotaService = new();

        // Mascota actualmente cargada en el control
        private Mascota mascotaActual;

        /// <summary>
        /// Constructor del UserControl.
        /// Recibe la mascota que se va a mostrar y la establece como DataContext.
        /// </summary>
        public UC_DatosMascota(Mascota mascota)
        {
            InitializeComponent();

            // Guardamos la mascota actual en memoria
            this.mascotaActual = mascota;

            // Enlazamos la mascota al DataContext para binding en XAML
            this.DataContext = mascotaActual;
        }

        /// <summary>
        /// Evento del botón "Editar mascota".
        /// Abre la ventana de edición y refresca los datos si se guardan cambios.
        /// </summary>
        private void BtnEditarMascota_Click(object sender, RoutedEventArgs e)
        {
            // Si el usuario es veterinario (rol 2), no puede editar
            if (Sesion.UsuarioActual!.IdRol == 2)
            {
                MessageBox.Show("No tienes permisos para editar esta información.");
                return;
            }

            try
            {
                WindowMascota ventanaEditar = new(mascotaActual)
                {
                    Owner = Window.GetWindow(this)
                };

                // Mostramos la ventana de forma modal
                bool? resultado = ventanaEditar.ShowDialog();

                // Si el usuario guardó cambios, recargamos la mascota desde BD
                if (resultado == true)
                {
                    mascotaActual = mascotaService.ObtenerPorId(mascotaActual.IdMascota)!;

                    // Actualizamos el DataContext para refrescar la vista
                    this.DataContext = mascotaActual;
                }
            }
            catch (Exception ex)
            {
                // Mostramos cualquier error ocurrido durante la edición
                MessageBox.Show("Error al editar: " + ex.Message);
            }
        }

        /// <summary>
        /// Evento del hyperlink del dueño.
        /// Abre la ficha del cliente asociado a la mascota.
        /// </summary>
        private void HyperlinkDueno_Click(object sender, RoutedEventArgs e)
        {
            // Verificamos que la mascota y el cliente existan
            if (mascotaActual != null && mascotaActual.IdCliente > 0)
            {
                // Abrimos la ventana de ficha del cliente (dueño)
                WindowFichaCliente ventanaDueno = new(mascotaActual.IdCliente)
                {
                    Owner = Window.GetWindow(this)
                };

                // Mostramos la ventana
                ventanaDueno.ShowDialog();
            }
        }
    }
}