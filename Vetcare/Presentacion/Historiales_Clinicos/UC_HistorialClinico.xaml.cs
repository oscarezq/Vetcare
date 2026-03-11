using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Negocio;
using Vetcare.Entidades;
using System.Collections.Generic;
using Vetcare.Service;
using Vetcare.Presentacion.HistorialesClinicos;

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
                dgHistorial.ItemsSource = lista;
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
    }
}