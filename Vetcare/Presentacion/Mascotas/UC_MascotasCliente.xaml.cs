using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Mascotas;

namespace Vetcare.Presentacion.Clientes
{
    public partial class UC_MascotasCliente : UserControl
    {
        private MascotaService mascotaService;
        private Cliente _clienteActual;

        public UC_MascotasCliente(Cliente cliente)
        {
            InitializeComponent();
            mascotaService = new MascotaService();
            _clienteActual = cliente;
            CargarMascotas();
        }

        private void CargarMascotas()
        {
            try
            {
                List<Mascota> mascotas = mascotaService.ObtenerPorCliente(_clienteActual.IdCliente);
                dgMascotas.ItemsSource = mascotas;

                // Lógica de visibilidad
                bool tieneMascotas = mascotas != null && mascotas.Count > 0;
                dgMascotas.Visibility = tieneMascotas ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las mascotas: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Este evento ahora maneja tanto el clic en el nombre (Hyperlink) 
        /// como el clic en el botón verde (Ver ficha)
        /// </summary>
        private void HyperlinkMascota_Click(object sender, RoutedEventArgs e)
        {
            // Usamos FrameworkContentElement para el Hyperlink o FrameworkElement para el Button
            // DataContext contiene la mascota de la fila actual
            if (sender is FrameworkElement element && element.DataContext is Mascota mascota)
            {
                AbrirFichaMascota(mascota);
            }
            else if (sender is Hyperlink hl && hl.DataContext is Mascota mascotaHl)
            {
                AbrirFichaMascota(mascotaHl);
            }
        }

        private void dgMascotas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mascotaSeleccionada = dgMascotas.SelectedItem as Mascota;
            if (mascotaSeleccionada != null)
            {
                AbrirFichaMascota(mascotaSeleccionada);
            }
        }

        private void AbrirFichaMascota(Mascota mascota)
        {
            WindowFichaMascota ficha = new WindowFichaMascota(mascota.IdMascota);
            ficha.Owner = Window.GetWindow(this);
            ficha.ShowDialog();

            // Recargamos por si se editó algún dato en la ficha
            CargarMascotas();
        }

        private void btnAnadirMascota_Click(object sender, RoutedEventArgs e)
        {
            // Verificación de seguridad para Rol 2
            if (Sesion.UsuarioActual?.IdRol == 2)
            {
                MessageBox.Show("No tiene permisos para añadir mascotas.", "Acceso denegado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            WindowMascota ventanaNueva = new WindowMascota();
            ventanaNueva.Owner = Window.GetWindow(this);

            // Configuramos los datos del dueño automáticamente
            ventanaNueva.txtNombreDueño.Text = $"{_clienteActual.Nombre} {_clienteActual.Apellidos} ({_clienteActual.NumDocumento})";
            ventanaNueva.txtNombreDueño.IsEnabled = false;
            ventanaNueva.btnSeleccionarDueño.IsEnabled = false;
            ventanaNueva.AsignarDueno(_clienteActual);

            ventanaNueva.ShowDialog();
            CargarMascotas();
        }
    }
}