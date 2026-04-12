using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Negocio;
using Vetcare.Entidades;
using System.Collections.Generic;
using Vetcare.Service;
using Vetcare.Presentacion.HistorialesClinicos;
using Microsoft.Win32;
using QuestPDF.Fluent;

namespace Vetcare.Presentacion.Mascotas
{
    public partial class UC_HistorialClinico : UserControl
    {
        private HistorialClinicoService historialService = new HistorialClinicoService();
        private CitaService citaService = new CitaService();
        private int idMascotaActual;

        public UC_HistorialClinico(int idMascota)
        {
            InitializeComponent();
            this.idMascotaActual = idMascota;
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            try
            {
                List<HistorialClinico> lista = historialService.ObtenerPorMascota(idMascotaActual);

                if (lista != null && lista.Count > 0)
                {
                    // Hay datos: Mostrar tabla, ocultar mensaje
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
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button).DataContext is HistorialClinico historial)) return;

            try
            {
                // 1. Buscamos la cita solo si existe (manejando el nullable int? idCita)
                Cita citaAsociada = null;
                if (historial.IdCita.HasValue && historial.IdCita.Value > 0)
                {
                    citaAsociada = citaService.ObtenerPorId(historial.IdCita.Value);
                }

                // 2. Abrimos la ventana pasando AMBOS o solo el historial
                // Es mejor que WindowConsulta tenga un constructor que acepte el historial
                WindowConsulta ventana = new WindowConsulta(historial, citaAsociada);

                ventana.Owner = Window.GetWindow(this);
                ventana.ShowDialog();

                // Opcional: Si la ventana permitió editar (aunque sea solo lectura, a veces se cambia algo)
                // puedes refrescar la tabla al cerrar
                // CargarHistorial(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo cargar el detalle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnNuevoRegistro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Abrimos la ventana SIN cita
                WindowConsulta ventana = new WindowConsulta(null);

                ventana.Owner = Window.GetWindow(this);
                ventana.ShowDialog();

                // Recargar historial tras cerrar
                CargarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear registro: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Obtener los datos necesarios
                List<HistorialClinico> lista = historialService.ObtenerPorMascota(idMascotaActual);

                // Necesitamos el objeto Mascota completo para el PDF
                // Asumo que tienes un MascotaService o DAO para traerlo por ID
                MascotaService mascotaService = new MascotaService();
                Mascota mascota = mascotaService.ObtenerPorId(idMascotaActual);

                if (mascota == null)
                {
                    MessageBox.Show("No se pudo obtener la información de la mascota.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 2. Configurar el diálogo para guardar archivo
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    FileName = $"Historial_{mascota.Nombre}_{DateTime.Now:yyyyMMdd}.pdf",
                    Title = "Guardar Historial Clínico"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // 3. Generar el documento usando QuestPDF
                    var documento = new HistorialDocumento(mascota, lista);
                    documento.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show("¡Historial exportado con éxito!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Opcional: Abrir el PDF automáticamente
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}