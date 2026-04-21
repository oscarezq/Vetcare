using System;
using System.Windows;
using System.Windows.Controls;
using QuestPDF.Fluent;
using Vetcare.Entidades;
using Vetcare.Negocio.Informes;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.HistorialesClinicos
{
    /// <summary>
    /// Ventana encargada de crear, visualizar y editar consultas veterinarias
    /// asociadas a una cita y su historial clínico.
    /// </summary>
    public partial class WindowConsulta : Window
    {
        // Cita que se está consultando
        private readonly Cita citaActual; 
        // Historial (en caso de edición)
        private readonly HistorialClinico? historialEdicion; 

        // Servicio de historial clínico
        private readonly HistorialClinicoService historialService = new(); 
        // Servicio de citas
        private readonly CitaService citaService = new(); 

        // Indica si la ventana está en modo edición
        private readonly bool esModoEdicion = false; 

        /// <summary>
        /// Abre la ventana para registrar una nueva consulta desde una cita
        /// </summary>
        public WindowConsulta(Cita cita)
        {
            InitializeComponent();

            citaActual = cita;
            esModoEdicion = false;

            ConfigurarInterfaz();
            CargarDatosCita();
        }

        /// <summary>
        /// Abre la ventana para ver o editar una consulta ya registrada
        /// </summary>
        public WindowConsulta(HistorialClinico historial, Cita citaAsociada)
        {
            InitializeComponent();

            historialEdicion = historial;
            citaActual = citaAsociada;
            esModoEdicion = true;

            ConfigurarInterfaz();
            CargarDatosEdicion();
        }

        /// <summary>
        /// Configura la interfaz según el modo (edición o creación)
        /// </summary>
        private void ConfigurarInterfaz()
        {
            if (esModoEdicion)
            {
                // Cambiar título
                lblTitulo.Text = "CONSULTA REGISTRADA";

                // Ocultar botón guardar inicialmente
                btnGuardar.Visibility = Visibility.Collapsed;

                // Mostrar botón editar solo si el usuario tiene permisos
                if (ValidarPermisosEdicion())
                    btnEditar.Visibility = Visibility.Visible;
                else
                    btnEditar.Visibility = Visibility.Collapsed;

                // Bloqueos de campos en modo lectura
                chkNoPesado.IsEnabled = false;
                BloquearCampos();

                // Mostrar botón imprimir solo si la cita está completada
                if (citaActual != null && citaActual.Estado == "Completada")
                    btnImprimir.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Valida si el usuario logueado puede editar la consulta
        /// </summary>
        private bool ValidarPermisosEdicion()
        {
            var usuarioLogueado = Sesion.UsuarioActual;

            if (usuarioLogueado == null)
                return false;

            // Admin tiene acceso total
            if (usuarioLogueado.IdRol == 1)
                return true;

            // Veterinario solo puede editar si es el asignado
            if (usuarioLogueado.IdRol == 2)
                return usuarioLogueado.IdVeterinario == citaActual.IdVeterinario;

            return false;
        }

        /// <summary>
        /// Bloquea los campos del formulario (solo lectura)
        /// </summary>
        private void BloquearCampos()
        {
            SetReadOnly(txtPeso);
            SetReadOnly(txtDiagnostico);
            SetReadOnly(txtTratamiento);
            SetReadOnly(txtObservaciones);
        }

        /// <summary>
        /// Activa los campos para edición
        /// </summary>
        private void ActivarCampos()
        {
            SetEditable(txtPeso);
            SetEditable(txtDiagnostico);
            SetEditable(txtTratamiento);
            SetEditable(txtObservaciones);
        }

        /// <summary>
        /// Pone un TextBox en modo solo lectura
        /// </summary>
        private static void SetReadOnly(TextBox tb)
        {
            tb.IsReadOnly = true;
        }

        /// <summary>
        /// Permite editar un TextBox
        /// </summary>
        private static void SetEditable(TextBox tb)
        {
            tb.IsReadOnly = false;
        }

        /// <summary>
        /// Carga información básica de la cita
        /// </summary>
        private void CargarDatosCita()
        {
            txtMascotaDisplay.Text = citaActual.NombreMascota;
            txtVeterinarioDisplay.Text = citaActual.NombreVeterinario;
            txtFechaDisplay.Text = citaActual.FechaHora.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Carga datos del historial cuando se edita una consulta
        /// </summary>
        private void CargarDatosEdicion()
        {
            CargarDatosCita();

            txtPeso.Text = historialEdicion!.Peso?.ToString();
            txtDiagnostico.Text = historialEdicion.Diagnostico;
            txtTratamiento.Text = historialEdicion.Tratamiento;
            txtObservaciones.Text = historialEdicion.Observaciones;
        }

        /// <summary>
        /// Activa modo edición en la interfaz
        /// </summary>
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            btnGuardar.Visibility = Visibility.Visible;
            btnEditar.Visibility = Visibility.Collapsed;

            chkNoPesado.IsEnabled = true;

            ActivarCampos();
        }

        /// <summary>
        /// Guarda o actualiza el historial clínico
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
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

            // Crear o reutilizar historial
            HistorialClinico? historial = esModoEdicion ? historialEdicion : new HistorialClinico();

            // Asignación de datos
            historial!.IdMascota = citaActual.IdMascota;
            historial.IdVeterinario = citaActual.IdVeterinario;
            historial.IdCita = citaActual.IdCita;
            historial.Diagnostico = txtDiagnostico.Text;
            historial.Tratamiento = txtTratamiento.Text;
            historial.Observaciones = txtObservaciones.Text;

            historial.Peso = string.IsNullOrWhiteSpace(txtPeso.Text)
                ? null
                : Convert.ToDecimal(txtPeso.Text);

            // Insertar o actualizar
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
        }

        /// <summary>
        /// Desactiva el campo de peso
        /// </summary>
        private void ChkNoPesado_Checked(object sender, RoutedEventArgs e)
        {
            txtPeso.Text = "";
            txtPeso.IsEnabled = false;
        }

        /// <summary>
        /// Reactiva el campo de peso
        /// </summary>
        private void ChkNoPesado_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPeso.IsEnabled = true;
        }

        /// <summary>
        /// Cierra la ventana sin guardar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Permite solo números y un decimal en el campo de peso
        /// </summary>
        private void NumericTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex("^[0-9]*[,.]?[0-9]*$");

            e.Handled = !regex.IsMatch(fullText);
        }

        /// <summary>
        /// Genera e imprime el informe en PDF de la consulta
        /// </summary>
        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (citaActual == null || historialEdicion == null)
                {
                    MessageBox.Show("No hay datos suficientes para generar el informe.");
                    return;
                }

                MascotaService mascotaService = new MascotaService();
                Mascota mascota = mascotaService.ObtenerPorId(citaActual.IdMascota);

                ClienteService clienteService = new ClienteService();
                Cliente cliente = clienteService.ObtenerPorId(citaActual.IdUsuarioDueno);

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