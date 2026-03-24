using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Mascotas.Especies;
using Vetcare.Presentacion.Mascotas.Razas;

namespace Vetcare.Presentacion
{
    public partial class WindowMascota : Window
    {
        private Mascota mascota;
        private MascotaService mascotaService = new MascotaService();
        private ClienteService clienteService = new ClienteService();
        private bool esEdicion = false;

        public WindowMascota()
        {
            InitializeComponent();
            mascota = new Mascota();
            esEdicion = false;

            lblTitulo.Text = "NUEVA MASCOTA";
            this.Title = "Nueva Mascota";
        }

        public WindowMascota(Mascota mascotaExistente)
        {
            InitializeComponent();
            mascota = mascotaExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR MASCOTA";
            this.Title = "Editar Mascota";

            CargarDatos();
        }

        public void AsignarDueno(Cliente cliente)
        {
            if (cliente == null) return;

            mascota.IdCliente = cliente.IdCliente;
            txtIdDueño.Text = cliente.IdCliente.ToString();
            txtNombreDueño.Text = $"{cliente.Nombre} {cliente.Apellidos} ({cliente.NumDocumento})";

            txtNombreDueño.FontStyle = FontStyles.Normal;
            txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            txtNombreDueño.FontWeight = FontWeights.SemiBold;
        }

        private void CargarDatos()
        {
            txtNombre.Text = mascota.Nombre;
            txtPeso.Text = mascota.Peso.ToString();
            dpNacimiento.SelectedDate = mascota.FechaNacimiento;

            txtChip.Text = mascota.NumeroChip;
            if (mascota.NumeroChip == "N/A")
            {
                chkNoChip.IsChecked = true;
            }

            // Especie
            txtIdEspecie.Text = mascota.IdEspecie.ToString();
            txtNombreEspecie.Text = mascota.NombreEspecie;
            txtNombreEspecie.Foreground = Brushes.Black;
            txtNombreEspecie.FontWeight = FontWeights.SemiBold;

            // Raza
            txtIdRaza.Text = mascota.IdRaza.ToString();
            txtNombreRaza.Text = mascota.NombreRaza;
            txtNombreRaza.Foreground = Brushes.Black;
            txtNombreRaza.FontWeight = FontWeights.SemiBold;

            btnSeleccionarRaza.IsEnabled = true;

            // Dueño
            txtIdDueño.Text = mascota.IdCliente.ToString();
            txtNombreDueño.Text = mascota.Dueno;
            txtNombreDueño.FontStyle = FontStyles.Normal;
            txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            txtNombreDueño.FontWeight = FontWeights.SemiBold;

            // Sexo
            foreach (ComboBoxItem item in cbSexo.Items)
            {
                if (item.Content.ToString() == mascota.Sexo)
                {
                    cbSexo.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                mascota.Nombre = txtNombre.Text.Trim();
                mascota.Peso = decimal.Parse(txtPeso.Text.Trim());
                mascota.NumeroChip = txtChip.Text.Trim();
                mascota.Sexo = ((ComboBoxItem)cbSexo.SelectedItem).Content.ToString();
                mascota.FechaNacimiento = dpNacimiento.SelectedDate.Value;

                mascota.IdCliente = int.Parse(txtIdDueño.Text);
                mascota.IdEspecie = int.Parse(txtIdEspecie.Text);
                mascota.IdRaza = int.Parse(txtIdRaza.Text);

                mascota.NombreEspecie = txtNombreEspecie.Text.Trim();
                mascota.NombreRaza = txtNombreRaza.Text.Trim();

                bool resultado;
                if (esEdicion)
                {
                    resultado = mascotaService.Actualizar(mascota);
                }
                else
                {
                    resultado = mascotaService.Insertar(mascota);
                }

                if (resultado)
                {
                    MessageBox.Show("Mascota guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar la mascota. Revise que los datos existan en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: Asegúrese de haber seleccionado Especie y Raza correctamente. Detalle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtChip.Text))
                return MostrarError("El número de chip es obligatorio.");

            string chip = txtChip.Text.Trim();
            if (string.IsNullOrWhiteSpace(chip))
            {
                return MostrarError("El número de chip es obligatorio (o marque 'No tiene').");
            }

            if (chip != "N/A")
            {
                if (chip.Length != 15 || !chip.All(char.IsDigit))
                    return MostrarError("El número de chip debe contener exactamente 15 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre de la mascota es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtNombreEspecie.Text))
                return MostrarError("La especie es obligatoria.");

            if (cbSexo.SelectedItem == null)
                return MostrarError("Debe seleccionar el sexo.");

            if (string.IsNullOrWhiteSpace(txtIdDueño.Text))
                return MostrarError("Debe seleccionar un dueño.");

            if (!decimal.TryParse(txtPeso.Text, out _))
                return MostrarError("El peso debe ser un valor numérico válido (ej: 12.5).");

            if (!dpNacimiento.SelectedDate.HasValue)
                return MostrarError("La fecha de nacimiento es obligatoria.");

            if (dpNacimiento.SelectedDate.Value > DateTime.Now)
                return MostrarError("La fecha de nacimiento no puede ser futura.");

            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBuscarDueño_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorCliente selector = new WindowSelectorCliente();
            selector.Owner = this;

            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;
                txtIdDueño.Text = cliente.IdCliente.ToString();
                txtNombreDueño.Text = $"{cliente.Nombre} {cliente.Apellidos} ({cliente.NumDocumento})";

                txtNombreDueño.FontStyle = FontStyles.Normal;
                txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
                txtNombreDueño.FontWeight = FontWeights.SemiBold;
            }
        }

        private void btnBuscarEspecie_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorEspecie selector = new WindowSelectorEspecie();
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                var especie = selector.EspecieSeleccionada;
                txtIdEspecie.Text = especie.IdEspecie.ToString();
                txtNombreEspecie.Text = especie.NombreEspecie;
                txtNombreEspecie.Foreground = Brushes.Black;
                mascota.IdEspecie = especie.IdEspecie;

                btnSeleccionarRaza.IsEnabled = true;

                txtIdRaza.Text = "";
                txtNombreRaza.Text = "Seleccionar raza...";
            }
        }

        private void btnBuscarRaza_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdEspecie.Text))
            {
                MessageBox.Show("Error: la especie no está cargada correctamente.");
                return;
            }

            int idEspecie = int.Parse(txtIdEspecie.Text);

            WindowSelectorRaza selector = new WindowSelectorRaza(idEspecie);
            selector.Owner = this;

            if (selector.ShowDialog() == true)
            {
                var raza = selector.RazaSeleccionada;
                txtIdRaza.Text = raza.IdRaza.ToString();
                txtNombreRaza.Text = raza.NombreRaza;
                txtNombreRaza.Foreground = Brushes.Black;
            }
        }

        private void chkNoChip_Checked(object sender, RoutedEventArgs e)
        {
            txtChip.Text = "N/A";
            txtChip.IsEnabled = false;
            txtChip.Background = new SolidColorBrush(Color.FromRgb(242, 244, 244));
        }

        private void chkNoChip_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtChip.Text == "N/A")
            {
                txtChip.Text = "";
            }
            txtChip.IsEnabled = true;
            txtChip.Background = Brushes.White;
        }
    }
}