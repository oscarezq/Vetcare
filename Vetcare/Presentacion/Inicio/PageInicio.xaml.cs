using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Datos;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;
using Vetcare.Presentacion.Servicios;

namespace Vetcare.Presentacion.Inicio
{
    /// <summary>
    /// Página principal (Dashboard) de la aplicación.
    /// Muestra información resumida del sistema.
    /// <br></br>
    /// Adapta su comportamiento y visualización según el rol del usuario (veterinario o administrador/recepción).
    /// </summary>
    public partial class PageInicio : Page
    {
        // Servicios de negocio para acceder a datos de citas
        private readonly CitaService _citaService = new();

        // Servicio de clientes
        private readonly ClienteService _clienteService = new();

        // Servicio de mascotas
        private readonly MascotaService _mascotaService = new();

        // Servicio de facturas
        private readonly FacturaService _facturaService = new();

        // Servicio de detalles de factura
        private readonly DetalleFacturaService _detalleFacturaService = new();

        // Servicio de veterinarios
        private readonly VeterinarioService _veterinarioService = new();

        /// <summary>
        /// Constructor de la página de inicio.
        /// Inicializa la interfaz y carga el dashboard.
        /// </summary>
        public PageInicio()
        {
            InitializeComponent();

            // Carga inicial de todos los datos del dashboard
            CargarPagina();
        }

        /// <summary>
        /// Carga toda la información del dashboard en función del usuario actual.
        /// Ajusta la interfaz según el rol del usuario.
        /// </summary>
        private void CargarPagina()
        {
            try
            {
                // Obtener usuario logueado en sesión
                var usuario = Sesion.UsuarioActual;

                // Si no hay usuario, no se carga nada
                if (usuario == null) return;

                // Contadores globales visibles para todos los roles
                txtTotalClientes.Text = _clienteService.ContarClientes().ToString();
                txtTotalMascotas.Text = _mascotaService.ContarMascotas().ToString();

                // ROL VETERINARIO
                if (usuario.IdRol == 2)
                {
                    // Obtener ID del veterinario asociado al usuario
                    int idVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuario.IdUsuario);

                    // Contar citas de hoy del veterinario
                    txtCitasHoy.Text = _citaService.ContarCitasHoyPorVeterinario(idVeterinario).ToString();

                    // Ocultar ingresos (no relevante para veterinario)
                    brdIngresos.Visibility = Visibility.Collapsed;
                    gridIndicadores.Columns = 3;

                    // Ajustar accesos rápidos para veterinario
                    btnNuevaMascota.Visibility = Visibility.Collapsed;
                    btnNuevoCliente.Visibility = Visibility.Collapsed;
                    btnAccesoFactura.Visibility = Visibility.Collapsed;
                    btnNuevoProducto.Visibility = Visibility.Visible;
                    btnNuevoServicio.Visibility = Visibility.Visible;
                    gridBotones.Columns = 3;

                    // Ocultar sección de facturas
                    brdFacturas.Visibility = Visibility.Collapsed;
                    colFacturas.Width = new GridLength(0);

                    // Cambiar título de agenda
                    txtTituloAgenda.Text = "Mis Citas de Hoy";

                    // Obtener citas del veterinario
                    var citas = _citaService.ObtenerCitasHoyPorVeterinario(idVeterinario);
                    dgCitas.ItemsSource = citas;

                    // Mostrar mensaje si no hay citas
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
                // ADMINISTRADOR / RECEPCIONISTA
                else
                {
                    // Contar citas generales del día
                    txtCitasHoy.Text = _citaService.ContarCitasHoy().ToString();

                    // Mostrar ingresos en dashboard
                    brdIngresos.Visibility = Visibility.Visible;
                    gridIndicadores.Columns = 4;

                    // Obtener ingresos del mes
                    txtIngresosHoy.Text = _facturaService.ObtenerIngresosMes().ToString("N2") + " €";

                    // Ajustar accesos rápidos
                    btnNuevaMascota.Visibility = Visibility.Visible;
                    btnNuevoCliente.Visibility = Visibility.Visible;
                    btnAccesoFactura.Visibility = Visibility.Visible;

                    btnNuevoProducto.Visibility = Visibility.Collapsed;
                    btnNuevoServicio.Visibility = Visibility.Collapsed;

                    gridBotones.Columns = 4;

                    // Mostrar sección de facturas
                    brdFacturas.Visibility = Visibility.Visible;
                    colFacturas.Width = new GridLength(1, GridUnitType.Star);

                    txtTituloAgenda.Text = "Agenda del Día";

                    // Obtener próximas citas
                    var citas = _citaService.ObtenerProximasCitas();
                    dgCitas.ItemsSource = citas;

                    // Mostrar mensaje si no hay citas
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

                    // Obtener facturas pendientes
                    var facturas = _facturaService.ObtenerFacturasPendientes();
                    dgFacturasPendientes.ItemsSource = facturas;

                    // Mostrar mensaje si no hay facturas pendientes
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
                // Mostrar error si falla la carga del dashboard
                MessageBox.Show("Error al cargar la página de inicio: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo producto.
        /// </summary>
        private void BtnNuevoProducto_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de producto
            WindowProducto ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar página si se guarda correctamente
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo servicio.
        /// </summary>
        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de servicio
            WindowServicio ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar si se guarda correctamente
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre el detalle de una factura pendiente.
        /// </summary>
        private void BtnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            // Obtener factura seleccionada desde el botón
            if (sender is Button btn && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    // Cargar detalles de la factura
                    facturaSeleccionada.Detalles =
                        _detalleFacturaService.ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    // Abrir ventana de detalle
                    WindowDetalleFactura detalleWin = new(facturaSeleccionada)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    // Recargar si se actualiza
                    if (detalleWin.ShowDialog() == true)
                        CargarPagina();
                }
                catch (Exception ex)
                {
                    // Mostrar error si falla la carga
                    MessageBox.Show("Error al cargar los detalles: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ficha de una cita seleccionada.
        /// </summary>
        private void BtnVerConsulta_Click(object sender, RoutedEventArgs e)
        {
            // Obtener cita seleccionada
            if (sender is Button btn && btn.DataContext is Cita cita)
            {
                // Abrir ventana de ficha de cita
                WindowFichaCita ventana = new(cita.IdCita)
                {
                    Owner = Window.GetWindow(this)
                };

                // Recargar si hay cambios
                if (ventana.ShowDialog() == true)
                    CargarPagina();
            }
        }

        /// <summary>
        /// Abre la ventana para crear una nueva cita.
        /// </summary>
        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de nueva cita
            WindowCita ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar dashboard
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva mascota.
        /// </summary>
        private void BtnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de mascota
            WindowMascota ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar si se guarda
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo cliente.
        /// </summary>
        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de cliente
            WindowCliente ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar dashboard
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva factura.
        /// </summary>
        private void BtnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de factura
            WindowFactura ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Recargar dashboard
            if (ventana.ShowDialog() == true)
                CargarPagina();
        }
    }
}