using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
            List<Mascota> mascotas = mascotaService.ObtenerPorCliente(_clienteActual.IdCliente);
            dgMascotas.ItemsSource = mascotas;

            // Lógica de visibilidad si no hay mascotas
            bool tieneMascotas = mascotas != null && mascotas.Count > 0;
            dgMascotas.Visibility = tieneMascotas ? Visibility.Visible : Visibility.Collapsed;
            // panelSinMascotas debe estar definido en el XAML del UC
            // panelSinMascotas.Visibility = tieneMascotas ? Visibility.Collapsed : Visibility.Visible;
        }

        private void HyperlinkMascota_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink hl && hl.DataContext is Mascota mascota)
            {
                WindowFichaMascota ficha = new WindowFichaMascota(mascota.IdMascota);
                ficha.Owner = Window.GetWindow(this);
                ficha.ShowDialog();
                CargarMascotas(); // Recargar por si se editó algo en la ficha
            }
        }

        private void btnAnadirMascota_Click(object sender, RoutedEventArgs e)
        {
            WindowMascota ventanaNueva = new WindowMascota();
            ventanaNueva.Owner = Window.GetWindow(this);

            // Configuración del dueño (tu lógica original)
            ventanaNueva.txtNombreDueño.Text = $"{_clienteActual.Nombre} {_clienteActual.Apellidos} ({_clienteActual.NumDocumento})";
            ventanaNueva.txtNombreDueño.IsEnabled = false;
            ventanaNueva.btnSeleccionarDueño.IsEnabled = false;
            ventanaNueva.AsignarDueno(_clienteActual);

            ventanaNueva.ShowDialog();
            CargarMascotas();
        }
    }
}