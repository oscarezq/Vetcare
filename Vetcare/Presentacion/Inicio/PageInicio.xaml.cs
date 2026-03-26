using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;

namespace Vetcare.Presentacion.Inicio
{
    public partial class PageInicio : Page
    {
        CitaDAO citaDAO = new CitaDAO();
        Conexion conexion = new Conexion();

        public PageInicio()
        {
            InitializeComponent();
            CargarDashboardReal();
        }

        private void CargarDashboardReal()
        {
            try
            {
                // Citas reales desde la BD
                List<Cita> lista = citaDAO.ObtenerTodas();
                dgCitas.ItemsSource = lista.Where(c => c.FechaHora.Date == DateTime.Today).ToList();

                // Conteos reales (Actualiza con tus tablas reales)
                txtTotalClientes.Text = GetCount("SELECT COUNT(*) FROM clientes");
                txtTotalMascotas.Text = GetCount("SELECT COUNT(*) FROM mascotas");
                txtCitasHoy.Text = lista.Count(c => c.FechaHora.Date == DateTime.Today).ToString();

                // Ingresos mes (Formateado a moneda)
                object ingresos = ExecuteScalar("SELECT SUM(total) FROM facturas WHERE MONTH(fecha_emision) = MONTH(CURRENT_DATE)");
                decimal total = ingresos != DBNull.Value ? Convert.ToDecimal(ingresos) : 0;
                txtIngresosHoy.Text = total.ToString("N2") + " €";

                //lblSaludo.Text = "Clínica VetCare - " + DateTime.Now.ToLongDateString();
            }
            catch (Exception ex)
            {
                //lblSaludo.Text = "Error al conectar con la base de datos.";
            }
        }

        private string GetCount(string sql)
        {
            return ExecuteScalar(sql).ToString();
        }

        private object ExecuteScalar(string sql)
        {
            using (MySqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(sql, con);
                return cmd.ExecuteScalar();
            }
        }

        // --- IMPLEMENTACIÓN ACCIONES RÁPIDAS ---

        private void btnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            WindowCita window = new WindowCita();
            window.Show();
        }

        private void btnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowMascota window = new WindowMascota();
            window.Show();
        }

        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente window = new WindowCliente();
            window.Show();
        }

        private void btnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            WindowFactura window = new WindowFactura();
            window.Show();
        }
    }
}