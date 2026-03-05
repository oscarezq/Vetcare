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
            txtNombreEspecie.Text = mascota.NombreEspecie;
            txtNombreRaza.Text = mascota.NombreRaza;
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
                // 1. Mapeo de datos básicos
                mascota.Nombre = txtNombre.Text.Trim();
                mascota.Peso = decimal.Parse(txtPeso.Text.Trim());
                mascota.NumeroChip = txtChip.Text.Trim();
                mascota.Sexo = ((ComboBoxItem)cbSexo.SelectedItem).Content.ToString();
                mascota.FechaNacimiento = dpNacimiento.SelectedDate.Value;

                // 2. ASIGNACIÓN DE IDS (Esto es lo que evita el error de Foreign Key)
                // Usamos los campos txtIdDueño, txtIdEspecie y txtIdRaza que se llenan en los selectores
                mascota.IdCliente = int.Parse(txtIdDueño.Text);
                mascota.IdEspecie = int.Parse(txtIdEspecie.Text); // <--- CAMBIO CRÍTICO
                mascota.IdRaza = int.Parse(txtIdRaza.Text);      // <--- CAMBIO CRÍTICO (El error venía de aquí)

                // 3. Mapeo de nombres (opcional, si tu entidad los usa para mostrar datos)
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

        /// <summary>
        /// Comprueba que todos los campos requeridos estén rellenos y tengan el formato correcto.
        /// </summary>
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtChip.Text))
                return MostrarError("El número de chip es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre de la mascota es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtNombreEspecie.Text))
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

        private void btnBuscarEspecie_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorEspecie selector = new WindowSelectorEspecie();
            selector.Owner = this; // Para que se centre respecto a esta ventana
            if (selector.ShowDialog() == true)
            {
                // Suponiendo que tu entidad se llama Especie y tiene Id y Nombre
                var especie = selector.EspecieSeleccionada;
                txtIdEspecie.Text = especie.IdEspecie.ToString();
                txtNombreEspecie.Text = especie.NombreEspecie;
                txtNombreEspecie.Foreground = System.Windows.Media.Brushes.Black;

                // Opcional: Limpiar la raza si cambia la especie
                txtIdRaza.Text = "";
                txtNombreRaza.Text = "Seleccionar raza...";
            }
        }

        private void btnBuscarRaza_Click(object sender, RoutedEventArgs e)
        {
            // Opcional: Podrías pasar el Id de la especie para filtrar razas de esa especie
            int idEspecie = string.IsNullOrEmpty(txtIdEspecie.Text) ? 0 : int.Parse(txtIdEspecie.Text);

            WindowSelectorRaza selector = new WindowSelectorRaza(idEspecie);
            selector.Owner = this;
            if (selector.ShowDialog() == true)
            {
                var raza = selector.RazaSeleccionada;
                txtIdRaza.Text = raza.IdRaza.ToString();
                txtNombreRaza.Text = raza.NombreRaza;
                txtNombreRaza.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
    }
}