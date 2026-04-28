using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using System.Collections.Generic;
using Vetcare.Presentacion.HistorialesClinicos;
using Microsoft.Win32;
using QuestPDF.Fluent;
using Vetcare.Negocio.Services;
using Vetcare.Negocio.Informes;

namespace Vetcare.Presentacion.Mascotas
{
    /// <summary>
    /// UserControl encargado de mostrar el historial clínico de una mascota,
    /// incluyendo la visualización de registros, acceso a detalles,
    /// creación de nuevos registros y exportación a PDF.
    /// </summary>
    public partial class UC_HistorialClinico : UserControl
    {
        // Servicio para gestionar los historiales clínicos
        private readonly HistorialClinicoService historialService = new();

        // Servicio para gestionar las citas asociadas
        private readonly CitaService citaService = new();

        // Servicio para gestionar las mascotas
        private readonly MascotaService mascotaService = new();

        // ID de la mascota actualmente cargada
        private readonly int idMascotaActual;

        /// <summary>
        /// Constructor del UserControl
        /// </summary>
        /// <param name="idMascota">ID de la mascota a mostrar</param>
        public UC_HistorialClinico(int idMascota)
        {
            InitializeComponent();

            // Guardamos el ID de la mascota
            this.idMascotaActual = idMascota;

            // Cargamos el historial clínico
            CargarHistorial();
        }

        /// <summary>
        /// Carga el historial clínico de la mascota desde la base de datos
        /// y actualiza la interfaz según haya o no datos.
        /// </summary>
        private void CargarHistorial()
        {
            try
            {
                // Obtenemos la lista de historiales
                List<HistorialClinico> lista = historialService.ObtenerPorMascota(idMascotaActual);

                if (lista != null && lista.Count > 0)
                {
                    // Hay datos: Mostrar tabla, ocultar mensaje vacío
                    dgHistorial.ItemsSource = lista;
                    dgHistorial.Visibility = Visibility.Visible;
                    pnlSinDatos.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // No hay datos: Ocultar tabla, mostrar mensaje
                    dgHistorial.Visibility = Visibility.Collapsed;
                    pnlSinDatos.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Error al cargar datos
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
        }

        /// <summary>
        /// Abre la ventana de detalle de un historial clínico
        /// </summary>
        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el historial desde el DataContext del botón
            if (((Button)sender).DataContext is not HistorialClinico historial) return;

            try
            {
                // Inicializamos la cita asociada
                Cita? citaAsociada;

                // Si el historial tiene cita asociada válida, la buscamos
                if (historial.IdCita.HasValue && historial.IdCita.Value > 0)
                {
                    citaAsociada = citaService.ObtenerPorId(historial.IdCita.Value);

                    if (citaAsociada != null)
                    {
                        // Abrimos la ventana de consulta pasando historial y cita
                        WindowConsulta ventana = new(historial, citaAsociada)
                        {
                            Owner = Window.GetWindow(this)
                        };

                        // Mostramos la ventana de forma modal
                        ventana.ShowDialog();

                        // Refrescar datos al cerrar
                        CargarHistorial();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cargar el detalle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Genera y exporta el historial clínico de la mascota a PDF
        /// </summary>
        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener información de la mascota
                Mascota? mascota = mascotaService.ObtenerPorId(idMascotaActual);

                // Obtener historial clínico
                List<HistorialClinico> lista = historialService.ObtenerPorMascota(idMascotaActual);

                // Validación de datos
                if (mascota == null)
                {
                    MessageBox.Show("No se pudo obtener la información de la mascota.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Configurar diálogo para guardar PDF
                SaveFileDialog? saveFileDialog = new()
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Historial_{mascota.Nombre}_{DateTime.Now:yyyyMMdd}.pdf",
                    Title = "Guardar Historial Clínico"
                };

                // Si el usuario selecciona una ruta
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Generar documento PDF con QuestPDF
                    var documento = new HistorialDocumento(mascota, lista);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    // Confirmación
                    MessageBox.Show("¡Historial exportado con éxito!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abrir automáticamente el PDF generado
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName)
                        { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}