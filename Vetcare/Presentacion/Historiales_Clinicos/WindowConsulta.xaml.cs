using System;
using System.Windows;
using System.Windows.Controls;
using QuestPDF.Fluent;
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

                // Solo mostramos el botón editar si cumple los requisitos
                if (ValidarPermisosEdicion())
                {
                    btnEditar.Visibility = Visibility.Visible;
                }
                else
                {
                    btnEditar.Visibility = Visibility.Collapsed;
                }

                chkNoPesado.IsEnabled = false;
                BloquearCampos();

                if (citaActual != null && citaActual.Estado == "Completada")
                {
                    btnImprimir.Visibility = Visibility.Visible;
                }
            }
        }

        private bool ValidarPermisosEdicion()
        {
            // 1. Obtener el usuario de la sesión global de tu App
            var usuarioLogueado = Sesion.UsuarioActual;

            if (usuarioLogueado == null) return false;

            // 2. Si es Administrador, tiene permiso total
            if (usuarioLogueado.IdRol == 1)
            {
                return true;
            }

            // 3. Si es Veterinario, comprobar si es el autor de la consulta o el asignado a la cita
            if (usuarioLogueado.IdRol == 2)
            {
                // Comparamos el ID del veterinario logueado con el de la cita
                // Nota: Asegúrate de que los IDs sean del mismo tipo (int, etc.)
                return usuarioLogueado.IdVeterinario == citaActual.IdVeterinario;
            }

            return false;
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
            this.DialogResult = true;
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

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual == null || historialEdicion == null)
                {
                    MessageBox.Show("No hay datos suficientes para generar el informe.");
                    return;
                }

                // Obtener mascota
                MascotaService mascotaService = new MascotaService();
                Mascota mascota = mascotaService.ObtenerPorId(citaActual.IdMascota);

                // Obtener cliente
                ClienteService clienteService = new ClienteService();
                Cliente cliente = clienteService.ObtenerPorId(citaActual.IdUsuarioDueno);

                // Guardar PDF
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "PDF (*.pdf)|*.pdf",
                    FileName = $"Consulta_{mascota.Nombre}_{citaActual.FechaHora:yyyyMMdd}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var documento = new InformeConsultaDocumento(mascota, cliente, citaActual, historialEdicion);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("Informe generado correctamente");

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar informe: " + ex.Message);
            }
        }
    }
}