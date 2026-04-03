using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;

namespace Vetcare.Presentacion.Inicio
{
    public partial class PageInicio : Page
    {
        Conexion conexion = new Conexion();
        CitaService _citaService = new CitaService();
        ClienteService _clienteService = new ClienteService();
        MascotaService _mascotaService = new MascotaService();
        FacturaService _facturaService = new FacturaService();

        public PageInicio()
        {
            InitializeComponent();
            CargarDashboardReal();
        }

        private void CargarDashboardReal()
        {
            try
            {
                // 1. Obtener citas
                List<Cita> citasHoy = _citaService.ObtenerProximasCitas();

                // 2. Asignar al DataGrid
                dgCitas.ItemsSource = citasHoy;

                // 3. Controlar visibilidad del mensaje de "No hay citas"
                if (citasHoy.Count == 0)
                {
                    dgCitas.Visibility = Visibility.Collapsed;
                    pnlSinCitas.Visibility = Visibility.Visible;
                }
                else
                {
                    dgCitas.Visibility = Visibility.Visible;
                    pnlSinCitas.Visibility = Visibility.Collapsed;
                }

                // --- Resto de tus conteos ---
                txtTotalClientes.Text = _clienteService.ContarClientes().ToString();
                txtTotalMascotas.Text = _mascotaService.ContarMascotas().ToString();
                txtCitasHoy.Text = _citaService.ContarCitasHoy().ToString();

                object ingresos = _facturaService.ObtenerIngresosMes();
                decimal total = ingresos != DBNull.Value ? Convert.ToDecimal(ingresos) : 0;
                txtIngresosHoy.Text = total.ToString("N2") + " €";
            }
            catch (Exception ex)
            {
                // Opcional: Mostrar mensaje de error en lugar de solo fallar en silencio
                MessageBox.Show("Error al cargar el dashboard: " + ex.Message);
            }
        }

        private void btnVerConsulta_Click(object sender, RoutedEventArgs e)
        {
            // Obtenemos el objeto Cita vinculado a la fila del botón pulsado
            var button = sender as Button;
            var citaSeleccionada = button.DataContext as Cita;

            if (citaSeleccionada != null)
            {
                WindowFichaCita window = new WindowFichaCita(citaSeleccionada.IdCita);
                window.ShowDialog();
            }
        }

        // --- IMPLEMENTACIÓN ACCIONES RÁPIDAS ---

        private void btnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            WindowCita window = new WindowCita();
            window.ShowDialog();
        }

        private void btnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowMascota window = new WindowMascota();
            window.ShowDialog();
        }

        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente window = new WindowCliente();
            window.ShowDialog();
        }

        private void btnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            WindowFactura window = new WindowFactura();
            window.ShowDialog();
        }
    }
}