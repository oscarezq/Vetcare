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
    /// <summary>
    /// Página principal (Dashboard) de la aplicación.
    /// Muestra información resumida del sistema.
    /// <br></br>
    /// Adapta su comportamiento y visualización según el rol del usuario (veterinario o administrador/recepción).
    /// </summary>
    public partial class PageInicio : Page
    {
        // Servicios de negocio para obtener datos del sistema.
        private readonly CitaService _citaService = new();
        private readonly ClienteService _clienteService = new();
        private readonly MascotaService _mascotaService = new();
        private readonly FacturaService _facturaService = new();
        private readonly DetalleFacturaService _detalleFacturaService = new();
        private readonly VeterinarioService _veterinarioService = new();

        /// <summary>
        /// Constructor de la página de inicio.
        /// Inicializa la vista y carga los datos del dashboard.
        /// </summary>
        public PageInicio()
        {
            InitializeComponent();
            CargarPagina();
        }

        /// <summary>
        /// Carga toda la información del dashboard en función del usuario actual.
        /// Muestra distintos datos y opciones según el rol (veterinario o admin/recepción).
        /// </summary>
        private void CargarPagina()
        {
            try
            {
                // Obtenemos el usuario con el que está activa la sesión
                var usuario = Sesion.UsuarioActual;
                if (usuario == null) return;

                // Contadores generales (visibles para todos los roles)
                txtTotalClientes.Text = _clienteService.ContarClientes().ToString();
                txtTotalMascotas.Text = _mascotaService.ContarMascotas().ToString();

                // Lógica según el rol del usuario
                // --- MODO VETERINARIO ---
                if (usuario.IdRol == 2) 
                {
                    int idVeterinario = _veterinarioService.ObtenerIdVeterinarioPorUsuario(usuario.IdUsuario);

                    // Número de citas sin cancelar que tiene hoy el veterinario
                    txtCitasHoy.Text = _citaService.ContarCitasHoyPorVeterinario(idVeterinario).ToString();

                    // Ocultamos ingresos (no relevante para veterinario)
                    brdIngresos.Visibility = Visibility.Collapsed;
                    gridIndicadores.Columns = 3;

                    // Accesos rápidos dispnibles
                    btnNuevaMascota.Visibility = Visibility.Collapsed;
                    btnNuevoCliente.Visibility = Visibility.Collapsed;
                    btnAccesoFactura.Visibility = Visibility.Collapsed;
                    btnNuevoProducto.Visibility = Visibility.Visible;
                    btnNuevoServicio.Visibility = Visibility.Visible;
                    gridBotones.Columns = 3;

                    // Tabla de citas (solo las propias del veterinario)
                    brdFacturas.Visibility = Visibility.Collapsed;
                    colFacturas.Width = new GridLength(0);
                    txtTituloAgenda.Text = "Mis Citas de Hoy";
                    var citas = _citaService.ObtenerCitasHoyPorVeterinario(idVeterinario);
                    dgCitas.ItemsSource = citas;

                    // Mostrar panel si no hay citas
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
                // --- MODO ADMINISTRADOR / RECEPCIONISTA ---
                else
                {
                    // Número total de citas que hay hoy sin cancelar
                    txtCitasHoy.Text = _citaService.ContarCitasHoy().ToString();

                    brdIngresos.Visibility = Visibility.Visible;
                    gridIndicadores.Columns = 4;

                    // Ingresos del mes
                    txtIngresosHoy.Text = _facturaService.ObtenerIngresosMes().ToString("N2") + " €";

                    // Accesos rápidos dispnibles
                    btnNuevaMascota.Visibility = Visibility.Visible;
                    btnNuevoCliente.Visibility = Visibility.Visible;
                    btnAccesoFactura.Visibility = Visibility.Visible;

                    btnNuevoProducto.Visibility = Visibility.Collapsed;
                    btnNuevoServicio.Visibility = Visibility.Collapsed;

                    gridBotones.Columns = 4;

                    // Tabla con todas las citas
                    brdFacturas.Visibility = Visibility.Visible;
                    colFacturas.Width = new GridLength(1, GridUnitType.Star);
                    txtTituloAgenda.Text = "Agenda del Día";
                    var citas = _citaService.ObtenerProximasCitas();
                    dgCitas.ItemsSource = citas;

                    // Mostrar panel si no hay citas
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

                    // Tabla con todas las facturas pendientes de pago
                    var facturas = _facturaService.ObtenerFacturasPendientes();
                    dgFacturasPendientes.ItemsSource = facturas;

                    // Mostrar panel si no hay facturas pendientes de pago
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
            WindowProducto ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo servicio.
        /// </summary>
        private void BtnNuevoServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowServicio ventana = new()
            {
                Owner = Window.GetWindow(this)
            };


            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre el detalle de una factura pendiente.
        /// </summary>
        private void BtnVerFactura_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Factura facturaSeleccionada)
            {
                try
                {
                    // Cargar líneas de factura
                    facturaSeleccionada.Detalles =
                        _detalleFacturaService.ObtenerDetallesPorFactura(facturaSeleccionada.IdFactura);

                    WindowDetalleFactura detalleWin = new(facturaSeleccionada)
                    {
                        Owner = Window.GetWindow(this)
                    };

                    if (detalleWin.ShowDialog() == true)
                        CargarPagina();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los detalles: " + ex.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Abre la ficha de una cita.
        /// </summary>
        private void BtnVerConsulta_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Cita cita)
            {
                WindowFichaCita ventana = new(cita.IdCita)
                {
                    Owner = Window.GetWindow(this)
                };

                if(ventana.ShowDialog() == true)
                    CargarPagina();
            }
        }

        /// <summary>
        /// Abre la ventana para crear una nueva cita.
        /// </summary>
        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            WindowCita ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if(ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva mascota.
        /// </summary>
        private void BtnNuevaMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowMascota ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear un nuevo cliente.
        /// </summary>
        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowCliente ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarPagina();
        }

        /// <summary>
        /// Abre la ventana para crear una nueva factura.
        /// </summary>
        private void BtnNuevaFactura_Click(object sender, RoutedEventArgs e)
        {
            WindowFactura ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (ventana.ShowDialog() == true)
                CargarPagina();
        }
    }
}