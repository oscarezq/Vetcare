using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Mascotas.Especies;
using Vetcare.Presentacion.Mascotas.Razas;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Ventana para crear o editar una mascota.
    /// Permite asignar especie, raza, dueño y datos clínicos básicos.
    /// </summary>
    public partial class WindowMascota : Window
    {
        private readonly Mascota mascota;
        private readonly MascotaService mascotaService = new();
        private readonly bool esEdicion = false;

        /// <summary>
        /// Constructor para nueva mascota.
        /// </summary>
        public WindowMascota()
        {
            InitializeComponent();

            // Creamos una nueva instancia vacía
            mascota = new Mascota();
            esEdicion = false;

            lblTitulo.Text = "NUEVA MASCOTA";
            this.Title = "Nueva Mascota";
        }

        /// <summary>
        /// Constructor para editar mascota existente.
        /// </summary>
        public WindowMascota(Mascota mascotaExistente)
        {
            InitializeComponent();

            // Cargamos mascota existente
            mascota = mascotaExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR MASCOTA";
            this.Title = "Editar Mascota";

            // Rellenamos campos UI
            CargarDatos();
        }

        /// <summary>
        /// Asigna un dueño seleccionado desde el selector de clientes.
        /// </summary>
        public void AsignarDueno(Cliente cliente)
        {
            if (cliente == null) return;

            mascota.IdCliente = cliente.IdCliente;
            txtIdDueño.Text = cliente.IdCliente.ToString();
            txtNombreDueño.Text = $"{cliente.Nombre} {cliente.Apellidos} ({cliente.NumDocumento})";

            // Estilizamos visualmente el campo
            txtNombreDueño.FontStyle = FontStyles.Normal;
            txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            txtNombreDueño.FontWeight = FontWeights.SemiBold;
        }

        /// <summary>
        /// Carga los datos de la mascota en los controles del formulario.
        /// </summary>
        private void CargarDatos()
        {
            txtNombre.Text = mascota.Nombre;
            txtPeso.Text = mascota.Peso.ToString();
            dpNacimiento.SelectedDate = mascota.FechaNacimiento;

            txtChip.Text = mascota.NumeroChip;

            // Si no tiene chip
            if (mascota.NumeroChip == "N/A")
                chkNoChip.IsChecked = true;

            // ESPECIE 
            txtIdEspecie.Text = mascota.IdEspecie.ToString();
            txtNombreEspecie.Text = mascota.NombreEspecie;
            txtNombreEspecie.Foreground = Brushes.Black;
            txtNombreEspecie.FontWeight = FontWeights.SemiBold;

            // RAZA
            txtIdRaza.Text = mascota.IdRaza.ToString();
            txtNombreRaza.Text = mascota.NombreRaza;
            txtNombreRaza.Foreground = Brushes.Black;
            txtNombreRaza.FontWeight = FontWeights.SemiBold;

            btnSeleccionarRaza.IsEnabled = true;

            // DUEÑO
            txtIdDueño.Text = mascota.IdCliente.ToString();
            txtNombreDueño.Text = mascota.Dueno;
            txtNombreDueño.FontStyle = FontStyles.Normal;
            txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            txtNombreDueño.FontWeight = FontWeights.SemiBold;

            // SEXO
            foreach (ComboBoxItem item in cbSexo.Items)
            {
                if (item.Content.ToString() == mascota.Sexo)
                {
                    cbSexo.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Guarda o actualiza la mascota en base de datos.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                // Asignación de datos desde UI
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

                // Insertar o actualizar según modo
                if (esEdicion)
                    resultado = mascotaService.Actualizar(mascota);
                else
                    resultado = mascotaService.Insertar(mascota);

                if (resultado)
                {
                    MessageBox.Show("Mascota guardada correctamente.");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar la mascota.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        /// <summary>
        /// Valida todos los campos obligatorios del formulario.
        /// </summary>
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtChip.Text))
                return MostrarError("El número de chip es obligatorio.");

            string chip = txtChip.Text.Trim();

            if (chip != "N/A")
            {
                if (chip.Length != 15 || !chip.All(char.IsDigit))
                    return MostrarError("El chip debe tener 15 dígitos numéricos.");
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtNombreEspecie.Text))
                return MostrarError("La especie es obligatoria.");

            if (cbSexo.SelectedItem == null)
                return MostrarError("Seleccione el sexo.");

            if (string.IsNullOrWhiteSpace(txtIdDueño.Text))
                return MostrarError("Seleccione un dueño.");

            if (!decimal.TryParse(txtPeso.Text, out _))
                return MostrarError("Peso inválido.");

            if (!dpNacimiento.SelectedDate.HasValue)
                return MostrarError("Fecha obligatoria.");

            if (dpNacimiento.SelectedDate > DateTime.Now)
                return MostrarError("Fecha no válida.");

            return true;
        }

        /// <summary>
        /// Muestra mensaje de error y detiene validación.
        /// </summary>
        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación");
            return false;
        }

        // ================= BOTONES =================

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Abre selector de cliente (dueño).
        /// </summary>
        private void btnBuscarDueño_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorCliente selector = new WindowSelectorCliente();
            selector.Owner = this;

            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;

                txtIdDueño.Text = cliente.IdCliente.ToString();
                txtNombreDueño.Text = $"{cliente.Nombre} {cliente.Apellidos} ({cliente.NumDocumento})";
            }
        }

        /// <summary>
        /// Selección de especie.
        /// </summary>
        private void btnBuscarEspecie_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorEspecie selector = new WindowSelectorEspecie();
            selector.Owner = this;

            if (selector.ShowDialog() == true)
            {
                var especie = selector.EspecieSeleccionada;

                txtIdEspecie.Text = especie.IdEspecie.ToString();
                txtNombreEspecie.Text = especie.NombreEspecie;

                btnSeleccionarRaza.IsEnabled = true;

                txtIdRaza.Text = "";
                txtNombreRaza.Text = "Seleccionar raza...";
            }
        }

        /// <summary>
        /// Selección de raza filtrada por especie.
        /// </summary>
        private void btnBuscarRaza_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtIdEspecie.Text, out int idEspecie))
            {
                WindowSelectorRaza selector = new WindowSelectorRaza(idEspecie);
                selector.Owner = this;

                if (selector.ShowDialog() == true)
                {
                    var raza = selector.RazaSeleccionada;

                    txtIdRaza.Text = raza.IdRaza.ToString();
                    txtNombreRaza.Text = raza.NombreRaza;
                }
            }
            else
            {
                MessageBox.Show("Seleccione primero una especie.");
            }
        }

        // ================= CHIP =================

        private void chkNoChip_Checked(object sender, RoutedEventArgs e)
        {
            txtChip.Text = "N/A";
            txtChip.IsEnabled = false;
        }

        private void chkNoChip_Unchecked(object sender, RoutedEventArgs e)
        {
            if (txtChip.Text == "N/A")
                txtChip.Text = "";

            txtChip.IsEnabled = true;
        }
    }
}