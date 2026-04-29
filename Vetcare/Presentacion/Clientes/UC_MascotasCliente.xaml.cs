using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Mascotas;

namespace Vetcare.Presentacion.Clientes
{
    public partial class UC_MascotasCliente : UserControl
    {
        // Servicio de negocio para operaciones con mascotas
        private readonly MascotaService mascotaService;

        // Cliente actualmente seleccionado en la vista
        private readonly Cliente _clienteActual;

        /// <summary>
        /// Constructor del control de usuario de mascotas del cliente.
        /// Inicializa servicios y carga las mascotas del cliente.
        /// </summary>
        public UC_MascotasCliente(Cliente cliente)
        {
            InitializeComponent();
            mascotaService = new MascotaService();
            _clienteActual = cliente;

            // Cargamos las mascotas del cliente al iniciar
            CargarMascotas();
        }

        /// <summary>
        /// Carga las mascotas del cliente actual desde la capa de negocio
        /// y actualiza la interfaz (DataGrid o panel vacío).
        /// </summary>
        private void CargarMascotas()
        {
            try
            {
                // Obtenemos la lista de mascotas del cliente
                List<Mascota> mascotas = mascotaService.ObtenerPorCliente(_clienteActual.IdCliente);

                // Asignamos la fuente de datos al DataGrid
                dgMascotas.ItemsSource = mascotas;

                // Comprobamos si hay mascotas para mostrar u ocultar panel vacío
                bool tieneMascotas = mascotas != null && mascotas.Count > 0;

                if (tieneMascotas)
                {
                    dgMascotas.Visibility = Visibility.Visible;
                    pnlSinMascotas.Visibility = Visibility.Collapsed;
                }
                else
                {
                    dgMascotas.Visibility = Visibility.Collapsed;
                    pnlSinMascotas.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Mensaje de error si falla la carga
                MessageBox.Show("Error al cargar las mascotas: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Evento para abrir la ficha de la mascota.
        /// Se puede ejecutar desde Hyperlink o botón.
        /// </summary>
        private void HyperlinkMascota_Click(object sender, RoutedEventArgs e)
        {
            // Caso 1: Click desde botón (FrameworkElement con DataContext)
            if (sender is FrameworkElement element && element.DataContext is Mascota mascota)
            {
                AbrirFichaMascota(mascota);
            }
            // Caso 2: Click desde Hyperlink
            else if (sender is Hyperlink hl && hl.DataContext is Mascota mascotaHl)
            {
                AbrirFichaMascota(mascotaHl);
            }
        }

        /// <summary>
        /// Evento de doble clic en el DataGrid para abrir ficha de mascota.
        /// </summary>
        private void DgMascotas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgMascotas.SelectedItem is Mascota mascotaSeleccionada)
                AbrirFichaMascota(mascotaSeleccionada);
        }

        /// <summary>
        /// Abre la ventana de ficha de la mascota seleccionada.
        /// </summary>
        private void AbrirFichaMascota(Mascota mascota)
        {
            // Abrimos ventana de ficha de mascota
            WindowFichaMascota ficha = new(mascota.IdMascota)
            {
                Owner = Window.GetWindow(this)
            };
            ficha.ShowDialog();

            // Recargamos datos por si hubo cambios en la ficha
            CargarMascotas();
        }

        /// <summary>
        /// Abre la ventana para añadir una nueva mascota al cliente.
        /// </summary>
        private void BtnAnadirMascota_Click(object sender, RoutedEventArgs e)
        {
            // Seguridad: si el usuario es rol 2 (veterinario), no puede añadir
            if (Sesion.UsuarioActual?.IdRol == 2)
            {
                MessageBox.Show("No tiene permisos para añadir mascotas.",
                    "Acceso denegado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Abrimos ventana de creación de mascota
            WindowMascota ventanaNueva = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Preasignamos el cliente como dueño
            ventanaNueva.txtNombreDueño.Text = $"{_clienteActual.Nombre} {_clienteActual.Apellidos} ({_clienteActual.NumDocumento})";
            ventanaNueva.txtNombreDueño.IsEnabled = false;
            ventanaNueva.btnSeleccionarDueño.IsEnabled = false;
            ventanaNueva.AsignarDueno(_clienteActual);

            ventanaNueva.ShowDialog();

            // Recargamos lista tras posible creación
            CargarMascotas();
        }
    }
}