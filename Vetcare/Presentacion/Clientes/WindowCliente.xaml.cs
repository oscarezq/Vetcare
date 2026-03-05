using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Ventana encargada de gestionar la creación y edición de clientes.
    /// </summary>
    public partial class WindowCliente : Window
    {
        // Cliente que se está creando o editando.
        private Cliente cliente;

        // Servicio encargado de la lógica de negocio de clientes.
        private ClienteService clienteService;

        // Booleano que indica si la ventana está en modo edición.
        private bool esEdicion = false;

        /// <summary>
        /// Constructor para crear un nuevo cliente.
        /// </summary>
        public WindowCliente()
        {
            InitializeComponent();

            clienteService = new ClienteService();

            cliente = new Cliente
            {
                FechaAlta = DateTime.Now
            };

            lblTitulo.Text = "Nuevo cliente";
            this.Title = "Nuevo cliente";
            txtFechaAlta.Text = DateTime.Now.ToString("dd/MM/yyyy");
            cmbTipoDocumento.SelectedIndex = 0;
        }

        /// <summary>
        /// Constructor para editar un cliente existente.
        /// </summary>
        /// <param name="clienteExistente">Cliente que se desea editar.</param>
        public WindowCliente(Cliente clienteExistente)
        {
            InitializeComponent();

            clienteService = new ClienteService();
            cliente = clienteExistente;
            esEdicion = true;

            lblTitulo.Text = "Editar cliente";
            this.Title = "Editar cliente";

            // Mostrar los datos del cliente en los controles
            CargarDatos();
        }

        /// <summary>
        /// Carga los datos del cliente en los controles del formulario.
        /// </summary>
        private void CargarDatos()
        {
            txtNumDocumento.Text = cliente.NumDocumento;
            txtNombre.Text = cliente.Nombre;
            txtApellidos.Text = cliente.Apellidos;
            txtTelefono.Text = cliente.Telefono;
            txtEmail.Text = cliente.Email;
            txtDireccion.Text = cliente.Direccion;
            txtFechaAlta.Text = cliente.FechaAlta.ToString("dd/MM/yyyy");

            // Seleccionar tipo de documento en el ComboBox
            if (!string.IsNullOrEmpty(cliente.TipoDocumento))
            {
                foreach (ComboBoxItem item in cmbTipoDocumento.Items)
                {
                    if (item.Content.ToString().Equals(cliente.TipoDocumento, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbTipoDocumento.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Evento que se ejecuta al pulsar el botón Guardar.
        /// Realiza validaciones completas antes de insertar o actualizar.
        /// </summary>
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Si no se cumple las validaciones, salimos del método
            if (!ValidarCampos())
                return;

            // Si se cumple las validaciones, asignamos los datos al cliente
            cliente.TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();
            cliente.NumDocumento = txtNumDocumento.Text.Trim().ToUpper();
            cliente.Nombre = txtNombre.Text.Trim();
            cliente.Apellidos = txtApellidos.Text.Trim();
            cliente.Telefono = txtTelefono.Text.Trim();
            cliente.Email = txtEmail.Text.Trim();
            cliente.Direccion = txtDireccion.Text.Trim();

            try
            {
                // Booleano para comprobar que ha salido todo bien
                bool resultado;

                if (esEdicion)
                {
                    // Si estamos en modo editar, actualizamos los datos del cliente en la base de datos
                    resultado = clienteService.Actualizar(cliente);
                }
                else
                {
                    // Si estamos en modo crear, se inserta el cliente en la base de datos
                    cliente.FechaAlta = DateTime.Now;
                    resultado = clienteService.Insertar(cliente);
                }

                if (resultado)
                {
                    // Si todo ha ido bien, informar al usuario
                    MessageBox.Show("Cliente guardado correctamente.", "Información",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // Cerrar esta ventana
                    this.Close();
                }
                else
                {
                    // Si no se ha podido insertar cliente, informar al usuario
                    MessageBox.Show("No se pudo guardar el cliente.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Si ha habido algún fallo, informar al usuario
                MessageBox.Show("Se ha producido un error: " + ex.Message, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida todos los campos del formulario.
        /// </summary>
        /// <returns>True si todos los datos son correctos.</returns>
        private bool ValidarCampos()
        {
            string tipoDoc = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();
            string numeroDoc = txtNumDocumento.Text.Trim();

            // Validación del tipo de documento
            if (string.IsNullOrWhiteSpace(numeroDoc))
                return MostrarError("El número de documento es obligatorio.");

            switch (tipoDoc)
            {
                case "DNI":
                    if (!Regex.IsMatch(numeroDoc, @"^\d{8}[A-Z]$"))
                        return MostrarError("El DNI debe tener 8 números y 1 letra. Ej: 12345678A");
                    break;
                case "NIE":
                    if (!Regex.IsMatch(numeroDoc, @"^[XYZ]\d{7}[A-Z]$"))
                        return MostrarError("El NIE debe comenzar con X, Y o Z seguido de 7 dígitos y una letra. Ej: X1234567A");
                    break;
                case "Pasaporte":
                    if (!Regex.IsMatch(numeroDoc, @"^[A-Z0-9]{5,9}$"))
                        return MostrarError("El pasaporte debe contener entre 5 y 9 caracteres alfanuméricos.");
                    break;
            }

            // Validaciones restantes
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre del cliente es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                return MostrarError("Los apellidos del cliente son obligatorios.");
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                return MostrarError("El teléfono del cliente es obligatorio.");
            if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{9}$"))
                return MostrarError("El teléfono debe contener exactamente 9 dígitos. Ej: 600123456");
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                return MostrarError("El email del cliente es obligatorio.");
            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return MostrarError("El formato del email no es válido. Ej: ejemplo@dominio.com");
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
                return MostrarError("La dirección del cliente es obligatoria.");

            return true;
        }

        /// <summary>
        /// Muestra un mensaje de error estándar.
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar.</param>
        /// <returns>Siempre devuelve false para simplificar validaciones.</returns>
        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        /// <summary>
        /// Evento que se ejecuta al pulsar el botón Cancelar.
        /// </summary>
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

