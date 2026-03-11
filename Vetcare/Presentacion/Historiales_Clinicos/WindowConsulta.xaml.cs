using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Service;

namespace Vetcare.Presentacion.HistorialesClinicos
{
    public partial class WindowConsulta : Window
    {
        private Cita citaActual;
        private HistorialClinico historialEdicion;
        private HistorialClinicoService historialService;
        private CitaService citaService;
        private bool esModoEdicion = false;

        // CONSTRUCTOR PARA CREAR (Recibe la cita de la que proviene)
        public WindowConsulta(Cita cita)
        {
            InitializeComponent();
            citaService = new CitaService();
            historialService = new HistorialClinicoService();

            this.citaActual = cita;
            this.esModoEdicion = false;

            ConfigurarInterfaz();
            CargarDatosCita();
        }

        // CONSTRUCTOR PARA EDITAR (Recibe el registro histórico ya existente)
        public WindowConsulta(HistorialClinico historial, Cita citaAsociada)
        {
            InitializeComponent();
            citaService = new CitaService();
            historialService = new HistorialClinicoService();

            this.historialEdicion = historial;
            this.citaActual = citaAsociada;
            this.esModoEdicion = true;

            ConfigurarInterfaz();
            CargarDatosEdicion();
        }

        private void ConfigurarInterfaz()
        {
            if (esModoEdicion)
            {
                this.Title = "Detalle de la Consulta";
                lblTitulo.Text = "CONSULTA REGISTRADA";
                btnGuardar.Visibility = Visibility.Collapsed;
                btnCancelar.Content = "Cerrar"; // Más natural para modo lectura

                // 1. Bloquear el CheckBox
                chkNoPesado.IsEnabled = false;

                // 2. Bloquear y "limpiar" visualmente los campos
                ConfigurarCampoLectura(txtPeso);
                ConfigurarCampoLectura(txtDiagnostico);
                ConfigurarCampoLectura(txtTratamiento);
                ConfigurarCampoLectura(txtObservaciones);
            }
            else
            {
                this.Title = "Registrar Consulta";
                lblTitulo.Text = "REGISTRAR NUEVA CONSULTA";
                btnGuardar.Visibility = Visibility.Visible;
            }
        }

        // Método auxiliar para que no se vean "raros" (gris feo) en edición
        private void ConfigurarCampoLectura(TextBox tb)
        {
            tb.IsReadOnly = true;
            tb.Focusable = false; // Evita que aparezca el cursor parpadeando
            tb.Background = System.Windows.Media.Brushes.Transparent;
            tb.BorderThickness = new Thickness(0, 0, 0, 1); // Solo una línea sutil abajo
            tb.BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#ECF0F1");
            tb.Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#2C3E50");
        }

        private void CargarDatosCita()
        {
            txtMascotaDisplay.Text = citaActual.NombreMascota;
            txtVeterinarioDisplay.Text = citaActual.NombreVeterinario;
            txtFechaDisplay.Text = citaActual.FechaHora.ToString("dd/MM/yyyy HH:mm");
        }

        private void CargarDatosEdicion()
        {
            CargarDatosCita(); // Muestra los datos de la cita arriba
            txtPeso.Text = historialEdicion.Peso?.ToString();
            txtDiagnostico.Text = historialEdicion.Diagnostico;
            txtTratamiento.Text = historialEdicion.Tratamiento;
            txtObservaciones.Text = historialEdicion.Observaciones;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            // Mapeo de datos (Nuevo o Existente)
            HistorialClinico historial = esModoEdicion ? historialEdicion : new HistorialClinico();

            historial.IdMascota = citaActual.IdMascota;
            historial.IdVeterinario = citaActual.IdVeterinario;
            historial.IdCita = citaActual.IdCita;
            historial.Diagnostico = txtDiagnostico.Text.Trim();
            historial.Tratamiento = txtTratamiento.Text.Trim();
            historial.Observaciones = txtObservaciones.Text.Trim();

            if (!string.IsNullOrWhiteSpace(txtPeso.Text))
                historial.Peso = Convert.ToDecimal(txtPeso.Text);
            else
                historial.Peso = null;

            try
            {
                bool resultado;
                if (esModoEdicion)
                {
                    resultado = historialService.Actualizar(historial);
                }
                else
                {
                    historial.FechaHora = DateTime.Now;
                    resultado = historialService.Insertar(historial);
                    if (resultado) citaService.ActualizarEstado(citaActual.IdCita, "Completada");
                }

                if (resultado)
                {
                    MessageBox.Show("Operación realizada con éxito", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("El diagnóstico es obligatorio");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPeso.Text) && !decimal.TryParse(txtPeso.Text, out _))
            {
                MessageBox.Show("El peso debe ser un número válido");
                return false;
            }

            return true;
        }

        private void chkNoPesado_Checked(object sender, RoutedEventArgs e)
        {
            txtPeso.Text = "0";
            txtPeso.IsEnabled = false; // Bloquea el cuadro para que no lo editen por error
            txtPeso.Background = System.Windows.Media.Brushes.GhostWhite;
        }

        private void chkNoPesado_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPeso.IsEnabled = true;
            txtPeso.Background = System.Windows.Media.Brushes.White;
            if (txtPeso.Text == "0") txtPeso.Clear(); // Limpia el 0 para que pongan el peso real
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}