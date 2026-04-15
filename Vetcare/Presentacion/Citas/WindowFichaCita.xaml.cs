using System;
using System.Windows;
using Microsoft.Win32;
using QuestPDF.Companion;
using QuestPDF.Fluent;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.HistorialesClinicos;
using Vetcare.Presentacion.Mascotas;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Service;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Lógica de interacción para la ventana de detalles y gestión de una cita médica.
    /// Permite la visualización de datos, cancelación de citas, gestión de consultas clínicas 
    /// y generación de documentos PDF (recordatorios y justificantes).
    /// </summary>
    public partial class WindowFichaCita : Window
    {
        // Capa de servicio para la persistencia y lógica de negocio de citas
        private readonly CitaService citaService;

        // Entidad que contiene los datos de la cita cargada actualmente
        private Cita? citaActual;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="WindowFichaCita"/>.
        /// </summary>
        /// <param name="idCita">Identificador único de la cita a consultar.</param>
        public WindowFichaCita(int idCita)
        {
            InitializeComponent();

            citaService = new CitaService();

            // Recuperación inicial de datos desde el origen de datos
            CargarCita(idCita);
        }

        /// <summary>
        /// Recupera la información de la cita desde la base de datos y actualiza el contexto de datos de la interfaz.
        /// </summary>
        /// <param name="idCita">ID de la cita a cargar.</param>
        private void CargarCita(int idCita)
        {
            try
            {
                // Consulta a la capa de negocio
                citaActual = citaService.ObtenerPorId(idCita);

                if (citaActual != null)
                {
                    // Asignación del modelo a la vista para habilitar el Data Binding
                    this.DataContext = citaActual;

                    // Ajuste de visibilidad y estados de controles según el flujo de trabajo
                    ActualizarInterfazSegunEstado();
                }
                else
                {
                    MessageBox.Show("No se pudo cargar la información de la cita.", "Error de Carga", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error crítico al recuperar la cita:\n" + ex.Message, "Error del Sistema", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Controlador de evento para el botón de registro o visualización de consultas.
        /// Determina si debe crear una nueva entrada clínica o permitir la edición de una existente.
        /// </summary>
        private void BtnRegistrarConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual == null) return;

                // Caso: Registro de una nueva consulta (Cita aún no atendida)
                if (citaActual.Estado == "Pendiente")
                {
                    WindowConsulta ventana = new(citaActual)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    if (ventana.ShowDialog() == true)
                    {
                        CargarCita(citaActual.IdCita);
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                // Caso: Visualización/Edición de registro clínico existente (Cita finalizada)
                else if (citaActual.Estado == "Completada" || citaActual.Estado == "Atendida")
                {
                    var historialService = new HistorialClinicoService();

                    // Obtención del registro clínico vinculado mediante el ID de la cita
                    HistorialClinico? historial = historialService.ObtenerPorIdCita(citaActual.IdCita);

                    if (historial != null)
                    {
                        WindowConsulta ventana = new(historial, citaActual)
                        {
                            Owner = Window.GetWindow(this)
                        };

                        if (ventana.ShowDialog() == true)
                        {
                            CargarCita(citaActual.IdCita);
                            this.DialogResult = true;
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se ha encontrado un historial clínico asociado a esta cita finalizada.",
                                        "Información", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la gestión de la consulta:\n" + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cierra la ventana actual de detalles de cita.
        /// </summary>
        private void BtnCerrar_Click(object sender, RoutedEventArgs e) => this.Close();

        /// <summary>
        /// Navega hacia la ficha detallada del propietario vinculado a la cita.
        /// </summary>
        private void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdUsuarioDueno > 0)
                {
                    var ventanaCliente = new WindowFichaCliente(citaActual.IdUsuarioDueno)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    ventanaCliente.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo acceder a la ficha del cliente:\n" + ex.Message,
                                "Error de Navegación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Navega hacia la ficha detallada de la mascota vinculada a la cita.
        /// </summary>
        private void BtnBuscarMascota_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdMascota > 0)
                {
                    var ventanaMascota = new WindowFichaMascota(citaActual.IdMascota)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    ventanaMascota.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo acceder a la ficha de la mascota:\n" + ex.Message,
                                "Error de Navegación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Navega hacia la ficha detallada del veterinario responsable de la cita.
        /// </summary>
        private void BtnBuscarVeterinario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual != null && citaActual.IdUsuarioVeterinario > 0)
                {
                    WindowFichaUsuario ventanaVet = new(citaActual.IdUsuarioVeterinario)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    ventanaVet.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo acceder a la ficha del profesional:\n" + ex.Message,
                                "Error de Navegación",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aplica restricciones de seguridad y lógica de negocio sobre los controles 
        /// de la interfaz basándose en el estado de la cita y privilegios de usuario.
        /// </summary>
        private void ActualizarInterfazSegunEstado()
        {
            if (citaActual == null) return;

            // Validación de permisos según el rol y asignación del usuario en sesión
            bool tienePermiso = ValidarPermisosAccion();

            // Restricción de edición para usuarios sin privilegios específicos en citas pendientes
            if (!tienePermiso && citaActual.Estado == "Pendiente")
            {
                btnRegistrarConsulta.IsEnabled = false;
                btnCancelarCita.IsEnabled = false;
            }
        }

        /// <summary>
        /// Verifica si el usuario actual tiene autoridad para realizar modificaciones.
        /// El acceso se permite a Administradores o al Veterinario asignado específicamente a la cita.
        /// </summary>
        /// <returns>Verdadero si el usuario está autorizado; de lo contrario, falso.</returns>
        private bool ValidarPermisosAccion()
        {
            var usuarioLogueado = Sesion.UsuarioActual;

            if (usuarioLogueado == null) return false;

            // Nivel de acceso: Administrador (ID Rol 1)
            if (usuarioLogueado.IdRol == 1)
                return true;

            // Nivel de acceso: Veterinario (ID Rol 2) - Requiere coincidencia de asignación
            if (usuarioLogueado.IdRol == 2)
                return usuarioLogueado.IdUsuario == citaActual!.IdUsuarioVeterinario;

            return false;
        }

        /// <summary>
        /// Ejecuta el proceso de cancelación de la cita tras confirmación del usuario.
        /// </summary>
        private void BtnCancelarCita_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("¿Está seguro de que desea cancelar esta cita de forma permanente?",
                                            "Confirmar Cancelación",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    citaActual!.Estado = "Cancelada";

                    // Actualización de estado en persistencia
                    bool actualizado = citaService.Actualizar(citaActual);

                    if (actualizado)
                    {
                        MessageBox.Show("La cita ha sido cancelada correctamente.", "Operación Exitosa",
                                        MessageBoxButton.OK, MessageBoxImage.Information);

                        CargarCita(citaActual.IdCita);
                        this.DialogResult = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al intentar cancelar la cita: " + ex.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Determina y ejecuta la generación del documento PDF apropiado basándose en el estado actual.
        /// </summary>
        private void BtnDescargarDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual == null) return;

                // Recordatorio para citas futuras o Justificante para pasadas
                if (citaActual.Estado == "Pendiente")
                    GenerarRecordatorio();
                else if (citaActual.Estado == "Completada")
                    GenerarJustificante();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar procesar el documento solicitado:\n" + ex.Message,
                                "Error de Documentación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Genera y exporta un archivo PDF con el recordatorio de la cita pendiente.
        /// </summary>
        private void GenerarRecordatorio()
        {
            try
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    FileName = $"Recordatorio_{citaActual!.NombreMascota}_{citaActual.FechaHora:yyyyMMdd}.pdf",
                    Title = "Exportar Recordatorio de Cita"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var documento = new RecordatorioDocumento(citaActual);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("El recordatorio se ha generado correctamente.", "PDF Creado");

                    // Apertura automática del documento generado mediante el visor predeterminado
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName)
                    { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el recordatorio PDF: " + ex.Message);
            }
        }

        /// <summary>
        /// Genera y exporta un archivo PDF con el justificante de asistencia para citas atendidas.
        /// </summary>
        private void GenerarJustificante()
        {
            try
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    FileName = $"Justificante_{citaActual!.NombreMascota}_{DateTime.Now:yyyyMMdd}.pdf",
                    Title = "Exportar Justificante Médico"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var documento = new JustificanteDocumento(citaActual);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("El justificante médico se ha generado correctamente.", "PDF Creado");

                    // Apertura automática del documento generado mediante el visor predeterminado
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName)
                    { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el justificante PDF: " + ex.Message);
            }
        }
    }
}