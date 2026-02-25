using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Ventana encargada de crear o editar una mascota en el sistema.
    /// </summary>
    public partial class WindowMascota : Window
    {
        // Mascota que se está creando o editando
        private Mascota mascota;

        // Servicios de la capa de negocio
        private MascotaService mascotaService = new MascotaService();
        private ClienteService clienteService = new ClienteService();

        // Booleano que indica si estamos editando o creando
        private bool esEdicion = false;

        /// <summary>
        /// Constructor para crear una nueva mascota.
        /// </summary>
        public WindowMascota()
        {
            InitializeComponent();

            mascota = new Mascota();
            esEdicion = false;

            lblTitulo.Text = "NUEVA MASCOTA";
            this.Title = "Nueva Mascota";

            // Ya no cargamos el combo de dueños porque usamos el botón selector
        }

        /// <summary>
        /// Constructor para editar una mascota existente.
        /// </summary>
        /// <param name="mascotaExistente">Objeto Mascota con los datos actuales.</param>
        public WindowMascota(Mascota mascotaExistente)
        {
            InitializeComponent();

            mascota = mascotaExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR MASCOTA";
            this.Title = "Editar Mascota";

            CargarDatos();
        }

        /// <summary>
        /// Método público para asignar un dueño a la mascota después de crear la ventana
        /// </summary>
        /// <param name="cliente"></param>
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

        /// <summary>
        /// Rellena los campos de la interfaz con los datos de la mascota (Modo Edición).
        /// </summary>
        private void CargarDatos()
        {
            txtNombre.Text = mascota.Nombre;
            txtEspecie.Text = mascota.Especie;
            txtRaza.Text = mascota.Raza;
            txtPeso.Text = mascota.Peso.ToString();
            dpNacimiento.SelectedDate = mascota.FechaNacimiento;
            txtChip.Text = mascota.NumeroChip;

            // Cargamos los datos del dueño en el botón/label
            txtIdDueño.Text = mascota.IdCliente.ToString();
            txtNombreDueño.Text = mascota.Dueno;
            txtNombreDueño.FontStyle = FontStyles.Normal;
            txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
            txtNombreDueño.FontWeight = FontWeights.SemiBold;

            // Seleccionar el sexo en el ComboBox comparando el contenido del texto
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
        /// Valida y persiste los datos de la mascota en la base de datos.
        /// </summary>
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                // Mapeo de datos desde los controles al objeto entidad
                mascota.Nombre = txtNombre.Text.Trim();
                mascota.Especie = txtEspecie.Text.Trim();
                mascota.Raza = txtRaza.Text.Trim();
                mascota.Peso = decimal.Parse(txtPeso.Text.Trim());
                mascota.NumeroChip = txtChip.Text.Trim();
                mascota.Sexo = ((ComboBoxItem)cbSexo.SelectedItem).Content.ToString();

                // Obtenemos el ID del cliente desde el TextBox oculto
                mascota.IdCliente = int.Parse(txtIdDueño.Text);
                mascota.FechaNacimiento = dpNacimiento.SelectedDate.Value;

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
                    this.DialogResult = true; // Para avisar a la ventana principal que refresque
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar la mascota. Revise los datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Comprueba que todos los campos requeridos estén rellenos y tengan el formato correcto.
        /// </summary>
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtChip.Text))
                return MostrarError("El número de chip es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre de la mascota es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtEspecie.Text))
                return MostrarError("La especie es obligatoria.");

            if (cbSexo.SelectedItem == null)
                return MostrarError("Debe seleccionar el sexo.");

            // Validamos el ID del dueño que viene del selector
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

        /// <summary>
        /// Muestra un aviso de advertencia al usuario.
        /// </summary>
        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        /// <summary>
        /// Cierra la ventana sin realizar cambios.
        /// </summary>
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Abre el selector de clientes para asignar un dueño a la mascota.
        /// </summary>
        private void btnBuscarDueño_Click(object sender, RoutedEventArgs e)
        {
            // 1. Abrimos el selector que creamos anteriormente
            WindowSelectorCliente selector = new WindowSelectorCliente();
            selector.Owner = this;

            // 2. Si el resultado es verdadero (hizo doble clic o dio a "Seleccionar")
            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;

                // 3. Rellenamos la interfaz
                txtIdDueño.Text = cliente.IdCliente.ToString();
                txtNombreDueño.Text = $"{cliente.Nombre} {cliente.Apellidos} ({cliente.NumDocumento})";

                // Estilo visual: quitamos el itálico y cambiamos color al seleccionar
                txtNombreDueño.FontStyle = FontStyles.Normal;
                txtNombreDueño.Foreground = new SolidColorBrush(Color.FromRgb(27, 38, 49));
                txtNombreDueño.FontWeight = FontWeights.SemiBold;
            }
        }
    }
}