using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;
using Vetcare.Presentacion.Servicios;

namespace Vetcare.Presentacion.Inicio
{
    public partial class PageInicio : Page
    {
        private CitaService _citaService = new CitaService();
        private ClienteService _clienteService = new ClienteService();
        private MascotaService _mascotaService = new MascotaService();
        private FacturaService _facturaService = new FacturaService();
        private VeterinarioService _veterinarioService = new VeterinarioService();

        public PageInicio()
        {
            InitializeComponent();
            CargarDashboardReal();
        }

        private void CargarDashboardReal()
        {
            try
            {
                var usuario = Sesion.UsuarioActual;
                if (usuario == null) return;

                // 1. CONTADORES GENERALES (Comunes para todos)
                txtTotalClientes.Text = _clienteService.ContarClientes().ToString();
                txtTotalMascotas.Text = _mascotaService.ContarMascotas().ToString();

                // 2. LÓGICA POR ROL
                if (usuario.IdRol == 2) // --- MODO VETERINARIO ---
                {
                    int idVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuario.IdUsuario);

                    // Indicadores
                    txtCitasHoy.Text = _citaService.ContarCitasHoyPorVeterinario(idVeterinario).ToString();
                    brdIngresos.Visibility = Visibility.Collapsed;
                    gridIndicadores.Columns = 3;

                    // Botones
                    btnNuevaMascota.Visibility = Visibility.Collapsed;
                    btnNuevoCliente.Visibility = Visibility.Collapsed;
                    btnAccesoFactura.Visibility = Visibility.Collapsed;
                    btnNuevoProducto.Visibility = Visibility.Visible;
                    btnNuevoServicio.Visibility = Visibility.Visible;
                    gridBotones.Columns = 3;

                    // Tabla de Citas Propias
                    brdFacturas.Visibility = Visibility.Collapsed;
                    colFacturas.Width = new GridLength(0);
                    txtTituloAgenda.Text = "Mis Citas de Hoy";

                    var citas = _citaService.ObtenerCitasHoyPorVeterinario(idVeterinario);
                    dgCitas.ItemsSource = citas;

                    // Lógica de visibilidad corregida
                    if (citas == null || citas.Count == 0)
                    {
                        pnlSinCitas.Visibility = Visibility.Visible;
                        dgCitas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        pnlSinCitas.Visibility = Visibility.Collapsed;
                        dgCitas.Visibility = Visibility.Visible;
                    }
                }
                else // --- MODO ADMIN / RECEPCIÓN ---
                {
                    // Indicadores
                    txtCitasHoy.Text = _citaService.ContarCitasHoy().ToString();
                    brdIngresos.Visibility = Visibility.Visible;
                    gridIndicadores.Columns = 4;
                    txtIngresosHoy.Text = _facturaService.ObtenerIngresosMes().ToString("N2") + " €";

                    // Botones
                    btnNuevaMascota.Visibility = Visibility.Visible;
                    btnNuevoCliente.Visibility = Visibility.Visible;
                    btnAccesoFactura.Visibility = Visibility.Visible;
                    btnNuevoProducto.Visibility = Visibility.Collapsed;
                    btnNuevoServicio.Visibility = Visibility.Collapsed;
                    gridBotones.Columns = 4;

                    // Tablas completas
                    brdFacturas.Visibility = Visibility.Visible;
                    colFacturas.Width = new GridLength(1, GridUnitType.Star);
                    txtTituloAgenda.Text = "Agenda del Día";

                    // --- Gestión de Citas ---
                    var citas = _citaService.ObtenerProximasCitas();
                    dgCitas.ItemsSource = citas;
                    if (citas == null || citas.Count == 0)
                    {
                        pnlSinCitas.Visibility = Visibility.Visible;
                        dgCitas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        pnlSinCitas.Visibility = Visibility.Collapsed;
                        dgCitas.Visibility = Visibility.Visible;
                    }

                    // --- Gestión de Facturas ---
                    var facturas = _facturaService.ObtenerFacturasPendientes();
                    dgFacturasPendientes.ItemsSource = facturas;
                    if (facturas == null || facturas.Count == 0)
                    {
                        pnlSinFacturas.Visibility = Visibility.Visible;
                        dgFacturasPendientes.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        pnlSinFacturas.Visibility = Visibility.Collapsed;
                        dgFacturasPendientes.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Dashboard: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- EVENTOS DE BOTONES ---

        private void btnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            new WindowProducto().ShowDialog();
            CargarDashboardReal();
        }

        private void btnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            new WindowServicio().ShowDialog();
            CargarDashboardReal();
        }

        private void btnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    DetalleFacturaDAO detalleDAO = new DetalleFacturaDAO();
                    facturaSeleccionada.Detalles = detalleDAO.ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    WindowDetalleFactura detalleWin = new WindowDetalleFactura(facturaSeleccionada);
                    detalleWin.ShowDialog();
                    CargarDashboardReal(); // Recargar por si se pagó la factura
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los detalles: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnVerConsulta_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Cita cita)
            {
                new WindowFichaCita(cita.IdCita).ShowDialog();
                CargarDashboardReal();
            }
        }

        private void btnNuevaCita_Click(object sender, RoutedEventArgs e) { new WindowCita().ShowDialog(); CargarDashboardReal(); }
        private void btnNuevaMascota_Click(object sender, RoutedEventArgs e) { new WindowMascota().ShowDialog(); CargarDashboardReal(); }
        private void btnNuevoCliente_Click(object sender, RoutedEventArgs e) { new WindowCliente().ShowDialog(); CargarDashboardReal(); }
        private void btnNuevaFactura_Click(object sender, RoutedEventArgs e) { new WindowFactura().ShowDialog(); CargarDashboardReal(); }
    }
}