using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Usuarios;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Lógica de interacción para WindowFichaCita.xaml
    /// </summary>
    public partial class WindowFichaCita : Window
    {
        private readonly CitaService citaService;
        private Cita citaActual;

        public WindowFichaCita(int idCita)
        {
            InitializeComponent();
            citaService = new CitaService();
            CargarCita(idCita);
        }

        /// <summary>
        /// Carga la cita desde la base de datos y la asigna al DataContext.
        /// </summary>
        private void CargarCita(int idCita)
        {
            try
            {
                citaActual = citaService.ObtenerPorId(idCita);

                if (citaActual != null)
                {
                    this.DataContext = citaActual;
                }
                else
                {
                    MessageBox.Show("No se pudo cargar la cita.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la cita:\n" + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Botón Editar.
        /// </summary>
        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCita ventanaEditar = new WindowCita(citaActual);
                ventanaEditar.Owner = this;

                if (ventanaEditar.ShowDialog() == true)
                {
                    CargarCita(citaActual.IdCita);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar la cita:\n" + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Botón Cerrar.
        /// </summary>
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Lupita Cliente → abre ficha del cliente.
        /// </summary>
        private void btnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdUsuarioDueno > 0)
                {
                    var ventanaCliente = new WindowFichaCliente(citaActual.IdUsuarioDueno);
                    ventanaCliente.Owner = this;
                    ventanaCliente.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ficha del cliente:\n" + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lupita Mascota → abre ficha de la mascota.
        /// </summary>
        private void btnBuscarMascota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdMascota > 0)
                {
                    var ventanaMascota = new WindowFichaMascota(citaActual.IdMascota);
                    ventanaMascota.Owner = this;
                    ventanaMascota.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ficha de la mascota:\n" + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lupita Veterinario → abre ficha del veterinario.
        /// </summary>
        private void btnBuscarVeterinario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdUsuarioVeterinario > 0)
                {
                    var ventanaVet = new WindowFichaUsuario(citaActual.IdUsuarioVeterinario);
                    ventanaVet.Owner = this;
                    ventanaVet.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ficha del veterinario:\n" + ex.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}