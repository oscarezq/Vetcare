using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Veterinarios;

namespace Vetcare.Presentacion.Citas
{
    public partial class WindowCita : Window
    {
        private Cita cita;
        private CitaService citaService = new CitaService();
        private MascotaService mascotaService = new MascotaService();
        private UsuarioService usuarioService = new UsuarioService();

        private bool esEdicion = false;
        private int idMascotaSeleccionada = 0;
        private int idVeterinarioSeleccionado = 0;

        public WindowCita()
        {
            InitializeComponent();
            cita = new Cita();
            dtpFecha.SelectedDate = DateTime.Now;

            AplicarRestriccionVeterinario();
        }

        public WindowCita(Cita citaExistente)
        {
            InitializeComponent();
            cita = citaExistente;
            esEdicion = true;
            lblTitulo.Text = "EDITAR CITA";
            this.Title = "Editar Cita";
            CargarDatos();
        }

        private void AplicarRestriccionVeterinario()
        {
            // Verificamos si existe sesión y si el rol es Veterinario (IdRol == 2 según tu lógica anterior)
            if (Sesion.UsuarioActual != null && Sesion.UsuarioActual.IdRol == 2)
            {
                // 1. Asignamos su ID de veterinario (usando su IdUsuario o IdVeterinario según tu DB)
                if (Sesion.UsuarioActual.IdVeterinario != null)
                    idVeterinarioSeleccionado = (int) Sesion.UsuarioActual.IdVeterinario;

                // 2. Actualizamos el texto visual con su nombre
                ActualizarVisualSelector(txtNombreVeterinario,
                    $"{Sesion.UsuarioActual.Nombre} {Sesion.UsuarioActual.Apellidos}");

                // 3. Deshabilitamos el botón para que no pueda cambiarlo
                btnSeleccionarVeterinario.IsEnabled = false;
            }
        }

        private void CargarDatos()
        {
            // Cargar Mascota y aplicar estilo visual
            idMascotaSeleccionada = cita.IdMascota;
            var mascota = mascotaService.ObtenerPorId(idMascotaSeleccionada);
            if (mascota != null) ActualizarVisualSelector(txtNombreMascota, mascota.Nombre);

            // Cargar Veterinario y aplicar estilo visual
            idVeterinarioSeleccionado = cita.IdUsuarioVeterinario;
            var vete = usuarioService.ObtenerPorId(idVeterinarioSeleccionado);
            if (vete != null) ActualizarVisualSelector(txtNombreVeterinario, $"{vete.Nombre} {vete.Apellidos}");

            txtDuracion.Text = cita.DuracionEstimada.ToString();
            dtpFecha.SelectedDate = cita.FechaHora.Date;
            txtHora.Text = cita.FechaHora.ToString("HH:mm");
            txtMotivo.Text = cita.Motivo;
            txtObservaciones.Text = cita.Observaciones;
        }

        private void btnBuscarMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorMascota selector = new WindowSelectorMascota();
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                idMascotaSeleccionada = selector.MascotaSeleccionada.IdMascota;
                ActualizarVisualSelector(txtNombreMascota, selector.MascotaSeleccionada.Nombre);
            }
        }

        private void btnBuscarVeterinario_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorVeterinario selector = new WindowSelectorVeterinario();
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                // CAMBIO AQUÍ: Debes capturar el IdVeterinario, no el IdUsuario
                idVeterinarioSeleccionado = selector.VeterinarioSeleccionado.IdVeterinario;

                ActualizarVisualSelector(txtNombreVeterinario,
                    $"{selector.VeterinarioSeleccionado.Nombre} {selector.VeterinarioSeleccionado.Apellidos}");
            }
        }
        private void ActualizarVisualSelector(TextBlock target, string texto)
        {
            target.Text = texto;
            target.FontStyle = FontStyles.Normal;
            target.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            target.FontWeight = FontWeights.SemiBold;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                cita.IdMascota = idMascotaSeleccionada;
                cita.IdVeterinario = idVeterinarioSeleccionado;
                cita.Estado = "Pendiente";
                cita.Motivo = txtMotivo.Text.Trim();
                cita.Observaciones = txtObservaciones.Text.Trim();
                cita.DuracionEstimada = int.Parse(txtDuracion.Text.Trim());

                DateTime fecha = dtpFecha.SelectedDate.Value;
                TimeSpan hora = TimeSpan.Parse(txtHora.Text);
                cita.FechaHora = fecha.Date + hora;

                bool res = esEdicion ? citaService.Actualizar(cita) : citaService.Insertar(cita);

                if (res)
                {
                    MessageBox.Show("Cita guardada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (idMascotaSeleccionada == 0) return MostrarError("Seleccione una mascota.");
            if (idVeterinarioSeleccionado == 0) return MostrarError("Seleccione un veterinario.");
            if (!dtpFecha.SelectedDate.HasValue) return MostrarError("Seleccione una fecha.");

            // Expresión regular para formato 24h (00:00 a 23:59)
            string patronHora = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";

            if (string.IsNullOrWhiteSpace(txtHora.Text))
                return MostrarError("La hora es obligatoria.");

            if (!Regex.IsMatch(txtHora.Text, patronHora))
                return MostrarError("Formato de hora inválido. Use el formato 24h (ej: 09:30 o 15:45).");

            if (string.IsNullOrWhiteSpace(txtMotivo.Text)) return MostrarError("El motivo es obligatorio.");

            if (!int.TryParse(txtDuracion.Text.Trim(), out int duracion) || duracion <= 0)
                return MostrarError("La duración debe ser un número entero mayor a 0.");

            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}