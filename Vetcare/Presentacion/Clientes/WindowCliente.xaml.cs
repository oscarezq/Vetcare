using System;
using System.Text.RegularExpressions;
using System.Windows;
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
            // Validación DNI
            if (string.IsNullOrWhiteSpace(txtNumDocumento.Text))
                return MostrarError("El DNI del cliente es obligatorio.");

            // Validación nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre del cliente es obligatorio.");

            // Validación apellidos
            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                return MostrarError("Los apellidos del cliente son obligatorios.");

            // Validación teléfono obligatorio y formato
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
                return MostrarError("El teléfono del cliente es obligatorio.");

            if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{9}$"))
                return MostrarError("El teléfono del cliente debe contener exactamente 9 dígitos. Ej: 600123456");

            // Validación email obligatorio y formato
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                return MostrarError("El email del cliente es obligatorio.");

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return MostrarError("El formato del email no es válido. Ej: ejemplo@dominio.com");

            // Validación dirección
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

