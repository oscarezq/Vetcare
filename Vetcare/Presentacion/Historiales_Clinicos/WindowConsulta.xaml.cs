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

        public WindowConsulta(Cita cita)
        {
            InitializeComponent();
            citaService = new CitaService();
            historialService = new HistorialClinicoService();

            citaActual = cita;
            esModoEdicion = false;

            ConfigurarInterfaz();
            CargarDatosCita();
        }

        public WindowConsulta(HistorialClinico historial, Cita citaAsociada)
        {
            InitializeComponent();
            citaService = new CitaService();
            historialService = new HistorialClinicoService();

            historialEdicion = historial;
            citaActual = citaAsociada;
            esModoEdicion = true;

            ConfigurarInterfaz();
            CargarDatosEdicion();
        }

        private void ConfigurarInterfaz()
        {
            if (esModoEdicion)
            {
                lblTitulo.Text = "CONSULTA REGISTRADA";
                btnGuardar.Visibility = Visibility.Collapsed;
                btnEditar.Visibility = Visibility.Visible;

                chkNoPesado.IsEnabled = false;

                BloquearCampos();
            }
        }

        private void BloquearCampos()
        {
            SetReadOnly(txtPeso);
            SetReadOnly(txtDiagnostico);
            SetReadOnly(txtTratamiento);
            SetReadOnly(txtObservaciones);
        }

        private void ActivarCampos()
        {
            SetEditable(txtPeso);
            SetEditable(txtDiagnostico);
            SetEditable(txtTratamiento);
            SetEditable(txtObservaciones);
        }

        private void SetReadOnly(TextBox tb)
        {
            tb.IsReadOnly = true;
        }

        private void SetEditable(TextBox tb)
        {
            tb.IsReadOnly = false;
        }

        private void CargarDatosCita()
        {
            txtMascotaDisplay.Text = citaActual.NombreMascota;
            txtVeterinarioDisplay.Text = citaActual.NombreVeterinario;
            txtFechaDisplay.Text = citaActual.FechaHora.ToString("dd/MM/yyyy");
        }

        private void CargarDatosEdicion()
        {
            CargarDatosCita();

            txtPeso.Text = historialEdicion.Peso?.ToString();
            txtDiagnostico.Text = historialEdicion.Diagnostico;
            txtTratamiento.Text = historialEdicion.Tratamiento;
            txtObservaciones.Text = historialEdicion.Observaciones;
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            btnGuardar.Visibility = Visibility.Visible;
            btnEditar.Visibility = Visibility.Collapsed;

            chkNoPesado.IsEnabled = true;

            ActivarCampos();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                MessageBox.Show("El diagnóstico es obligatorio");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                MessageBox.Show("El tratamiento es obligatorio");
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtPeso.Text) && !decimal.TryParse(txtPeso.Text, out _))
            {
                MessageBox.Show("Peso inválido");
                return;
            }

            HistorialClinico historial = esModoEdicion ? historialEdicion : new HistorialClinico();

            historial.IdMascota = citaActual.IdMascota;
            historial.IdVeterinario = citaActual.IdVeterinario;
            historial.IdCita = citaActual.IdCita;
            historial.Diagnostico = txtDiagnostico.Text;
            historial.Tratamiento = txtTratamiento.Text;
            historial.Observaciones = txtObservaciones.Text;

            historial.Peso = string.IsNullOrWhiteSpace(txtPeso.Text) ? null : Convert.ToDecimal(txtPeso.Text);

            if (!esModoEdicion)
            {
                historial.FechaHora = DateTime.Now;
                historialService.Insertar(historial);
                citaService.ActualizarEstado(citaActual.IdCita, "Completada");
            }
            else
            {
                historialService.Actualizar(historial);
            }

            MessageBox.Show("Guardado correctamente");
            this.Close();
        }

        private void chkNoPesado_Checked(object sender, RoutedEventArgs e)
        {
            txtPeso.Text = "";
            txtPeso.IsEnabled = false;
        }

        private void chkNoPesado_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPeso.IsEnabled = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Permite números y una sola coma o punto decimal
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Expresión regular: solo números y un separador decimal opcional
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]*[,.]?[0-9]*$");
            e.Handled = !regex.IsMatch(fullText);
        }
    }
}