using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.HistorialesClinicos;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Service;

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
                    ActualizarInterfazSegunEstado();
                }
                else
                {
                    MessageBox.Show("No se pudo cargar la cita.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la cita:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnRegistrarConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual == null) return;

                // 1. MODO CREACIÓN: Si la cita está pendiente, registramos una nueva consulta
                if (citaActual.Estado == "Pendiente")
                {
                    WindowConsulta ventana = new WindowConsulta(citaActual);
                    ventana.Owner = this;

                    if (ventana.ShowDialog() == true)
                    {
                        CargarCita(citaActual.IdCita); // Recargar para ver el cambio de estado a "Completada"
                        this.DialogResult = true;
                    }
                }
                // 2. MODO EDICIÓN/VISTA: Si ya está completada, buscamos el historial para editarlo
                else if (citaActual.Estado == "Completada" || citaActual.Estado == "Atendida")
                {
                    var historialService = new HistorialClinicoService();
                    // Buscamos el registro clínico asociado a esta cita
                    HistorialClinico historial = historialService.ObtenerPorIdCita(citaActual.IdCita);

                    if (historial != null)
                    {
                        // Usamos el nuevo constructor de edición que creamos antes
                        WindowConsulta ventana = new WindowConsulta(historial, citaActual);
                        ventana.Owner = this;

                        if (ventana.ShowDialog() == true)
                        {
                            CargarCita(citaActual.IdCita);
                            this.DialogResult = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el registro clínico de esta consulta.",
                                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al gestionar la consulta:\n" + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ActualizarInterfazSegunEstado()
        {
            if (citaActual == null) return;
            var bc = new System.Windows.Media.BrushConverter();

            switch (citaActual.Estado)
            {
                case "Pendiente":
                    borderEstado.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#F1C40F"); // Amarillo
                    btnRegistrarConsulta.Visibility = Visibility.Visible;
                    btnRegistrarConsulta.Content = "Registrar consulta";
                    btnRegistrarConsulta.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#27AE60");

                    // Mostrar botón cancelar solo si está pendiente
                    btnCancelarCita.Visibility = Visibility.Visible;
                    break;

                case "Atendida":
                case "Completada":
                    borderEstado.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#2ecc71"); // Verde
                    btnRegistrarConsulta.Visibility = Visibility.Visible;
                    btnRegistrarConsulta.Content = "Ver consulta";
                    btnRegistrarConsulta.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#3498DB");

                    btnCancelarCita.Visibility = Visibility.Collapsed;
                    break;

                case "Cancelada":
                    borderEstado.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#E74C3C"); // Rojo
                    btnRegistrarConsulta.Visibility = Visibility.Collapsed;
                    btnCancelarCita.Visibility = Visibility.Collapsed;
                    break;

                default:
                    borderEstado.Background = (System.Windows.Media.Brush)bc.ConvertFrom("#BDC3C7");
                    btnCancelarCita.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void btnCancelarCita_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro que desea cancelar esta cita?", "Confirmar",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Actualizamos el estado en el objeto y en la base de datos
                    citaActual.Estado = "Cancelada";
                    bool actualizado = citaService.Actualizar(citaActual); // Asegúrate que este método exista en tu Service

                    if (actualizado)
                    {
                        MessageBox.Show("Cita cancelada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarCita(citaActual.IdCita); // Recargamos para actualizar colores y botones
                        this.DialogResult = true; // Avisamos a la ventana padre que hubo cambios
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cancelar: " + ex.Message);
                }
            }
        }
    }
}