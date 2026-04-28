using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Veterinarios;

namespace Vetcare.Presentacion.Citas
{
    /// <summary>
    /// Ventana encargada de crear y editar citas del sistema.
    /// Permite seleccionar mascota, veterinario, fecha, hora y detalles de la cita.
    /// </summary>
    public partial class WindowCita : Window
    {
        // Objeto de la cita que se va a crear o editar
        private readonly Cita cita;

        // Servicios de negocio
        private readonly CitaService citaService = new();
        private readonly MascotaService mascotaService = new();
        private readonly UsuarioService usuarioService = new();

        // Indica si la ventana está en modo edición
        private readonly bool esEdicion = false;

        // IDs de las entidades seleccionadas
        private int idMascotaSeleccionada = 0;
        private int idVeterinarioSeleccionado = 0;

        /// <summary>
        /// Constructor para crear una nueva cita.
        /// </summary>
        public WindowCita()
        {
            InitializeComponent();

            cita = new Cita();

            // Fecha por defecto: hoy
            dtpFecha.SelectedDate = DateTime.Now;

            // Aplica restricción si el usuario es veterinario
            AplicarRestriccionVeterinario();
        }

        /// <summary>
        /// Constructor para editar una cita existente.
        /// </summary>
        public WindowCita(Cita citaExistente)
        {
            InitializeComponent();

            cita = citaExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR CITA";
            this.Title = "Editar Cita";

            CargarDatos();
        }

        /// <summary>
        /// Aplica restricciones si el usuario logueado es veterinario.
        /// Le asigna automáticamente su veterinario y bloquea cambios.
        /// </summary>
        private void AplicarRestriccionVeterinario()
        {
            // Verificamos si hay sesión activa y si el usuario es veterinario (Rol 2)
            if (Sesion.UsuarioActual != null && Sesion.UsuarioActual.IdRol == 2)
            {
                // Asignamos el veterinario del usuario logueado
                if (Sesion.UsuarioActual.IdVeterinario != null)
                    idVeterinarioSeleccionado = (int)Sesion.UsuarioActual.IdVeterinario;

                // Mostramos el nombre en la UI
                ActualizarVisualSelector(txtNombreVeterinario,
                    $"{Sesion.UsuarioActual.Nombre} {Sesion.UsuarioActual.Apellidos}");

                // Bloqueamos selección de veterinario
                btnSeleccionarVeterinario.IsEnabled = false;
            }
        }

        /// <summary>
        /// Carga los datos de una cita existente en la ventana.
        /// </summary>
        private void CargarDatos()
        {
            // Mascota asociada a la cita
            idMascotaSeleccionada = cita.IdMascota;
            var mascota = mascotaService.ObtenerPorId(idMascotaSeleccionada);
            if (mascota != null)
                ActualizarVisualSelector(txtNombreMascota, mascota.Nombre!);

            // Veterinario asociado a la cita
            idVeterinarioSeleccionado = cita.IdUsuarioVeterinario;
            var vete = usuarioService.ObtenerPorId(idVeterinarioSeleccionado);
            if (vete != null)
                ActualizarVisualSelector(txtNombreVeterinario, $"{vete.Nombre} {vete.Apellidos}");

            // Resto de campos
            txtDuracion.Text = cita.DuracionEstimada.ToString();
            dtpFecha.SelectedDate = cita.FechaHora.Date;
            txtHora.Text = cita.FechaHora.ToString("HH:mm");
            txtMotivo.Text = cita.Motivo;
            txtObservaciones.Text = cita.Observaciones;
        }

        /// <summary>
        /// Abre el selector de mascotas.
        /// </summary>
        private void BtnBuscarMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorMascota selector = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (selector.ShowDialog() == true)
            {
                idMascotaSeleccionada = selector.MascotaSeleccionada!.IdMascota;
                ActualizarVisualSelector(txtNombreMascota, selector.MascotaSeleccionada.Nombre!);
            }
        }

        /// <summary>
        /// Abre el selector de veterinarios.
        /// </summary>
        private void BtnBuscarVeterinario_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorVeterinario selector = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (selector.ShowDialog() == true)
            {
                // Guardamos el ID del veterinario seleccionado
                idVeterinarioSeleccionado = selector.VeterinarioSeleccionado!.IdVeterinario;

                ActualizarVisualSelector(txtNombreVeterinario,
                    $"{selector.VeterinarioSeleccionado.Nombre} {selector.VeterinarioSeleccionado.Apellidos}");
            }
        }

        /// <summary>
        /// Actualiza visualmente el selector en la UI.
        /// </summary>
        private static void ActualizarVisualSelector(TextBlock target, string texto)
        {
            target.Text = texto;
            target.FontStyle = FontStyles.Normal;
            target.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            target.FontWeight = FontWeights.SemiBold;
        }

        /// <summary>
        /// Guarda la cita (crear o actualizar).
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                cita.IdMascota = idMascotaSeleccionada;
                cita.IdUsuarioVeterinario = idVeterinarioSeleccionado;
                cita.Estado = "Pendiente";
                cita.Motivo = txtMotivo.Text.Trim();
                cita.Observaciones = txtObservaciones.Text.Trim();
                cita.DuracionEstimada = int.Parse(txtDuracion.Text.Trim());

                // Construcción de fecha + hora
                DateTime fecha = dtpFecha.SelectedDate!.Value;
                TimeSpan hora = TimeSpan.Parse(txtHora.Text);
                cita.FechaHora = fecha.Date + hora;

                bool res = esEdicion ? citaService.Actualizar(cita) : citaService.Insertar(cita);

                if (res)
                {
                    MessageBox.Show("Cita guardada con éxito.", "Éxito",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida los campos del formulario.
        /// </summary>
        private bool ValidarCampos()
        {
            if (idMascotaSeleccionada == 0)
                return MostrarError("Seleccione una mascota.");

            if (idVeterinarioSeleccionado == 0)
                return MostrarError("Seleccione un veterinario.");

            if (!dtpFecha.SelectedDate.HasValue)
                return MostrarError("Seleccione una fecha.");

            // Validación de formato de hora
            string patronHora = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";

            if (string.IsNullOrWhiteSpace(txtHora.Text))
                return MostrarError("La hora es obligatoria.");

            if (!Regex.IsMatch(txtHora.Text, patronHora))
                return MostrarError("Formato de hora inválido (HH:mm).");

            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                return MostrarError("El motivo es obligatorio.");

            if (!int.TryParse(txtDuracion.Text.Trim(), out int duracion) || duracion <= 0)
                return MostrarError("La duración debe ser mayor a 0.");

            return true;
        }

        /// <summary>
        /// Muestra un mensaje de error de validación.
        /// </summary>
        private static bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);

            return false;
        }

        /// <summary>
        /// Cierra la ventana.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}