using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Ventana encargada de crear o editar clientes.
    /// </summary>
    public partial class WindowCliente : Window
    {
        // Cliente que se está creando o editando
        private readonly Cliente cliente;

        // Servicio de negocio para operaciones CRUD
        private readonly ClienteService clienteService;

        // Indica si la ventana está en modo edición
        private readonly bool esEdicion = false;

        /// <summary>
        /// Constructor: crear nuevo cliente
        /// </summary>
        public WindowCliente()
        {
            InitializeComponent();

            clienteService = new ClienteService();

            // Se crea un cliente vacío con fecha actual
            cliente = new Cliente();

            // Configuración visual inicial
            lblTitulo.Text = "NUEVO CLIENTE";
            this.Title = "Nuevo cliente";

            // Selección por defecto del tipo de documento
            cmbTipoDocumento.SelectedIndex = 0;
        }

        /// <summary>
        /// Constructor: edición de cliente existente
        /// </summary>
        public WindowCliente(Cliente clienteExistente)
        {
            InitializeComponent();

            clienteService = new ClienteService();

            // Se asigna el cliente recibido
            cliente = clienteExistente;
            esEdicion = true;

            // Configuración visual de edición
            lblTitulo.Text = "EDITAR CLIENTE";
            this.Title = "Editar cliente";

            // Carga datos en pantalla
            CargarDatos();
        }

        /// <summary>
        /// Carga los datos del cliente en los controles de la ventana
        /// </summary>
        private void CargarDatos()
        {
            txtNumDocumento.Text = cliente.NumDocumento;
            txtNombre.Text = cliente.Nombre;
            txtApellidos.Text = cliente.Apellidos;
            txtTelefono.Text = cliente.Telefono;
            txtEmail.Text = cliente.Email;

            // Dirección
            txtCalle.Text = cliente.CalleDireccion;
            txtNumero.Text = cliente.NumeroDireccion;
            txtPisoPuerta.Text = cliente.PisoPuertaDireccion;
            txtCP.Text = cliente.CodigoPostalDireccion;
            txtLocalidad.Text = cliente.LocalidadDireccion;
            txtProvincia.Text = cliente.ProvinciaDireccion;

            // Selección del tipo de documento en ComboBox
            if (!string.IsNullOrEmpty(cliente.TipoDocumento))
            {
                foreach (ComboBoxItem item in cmbTipoDocumento.Items)
                {
                    if (item.Content.ToString()!
                        .Equals(cliente.TipoDocumento, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbTipoDocumento.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Evento click del botón guardar
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validación general
            if (!ValidarCampos())
                return;

            // ASIGNACIÓN DE DATOS
            cliente.TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();
            cliente.NumDocumento = txtNumDocumento.Text.Trim().ToUpper();
            cliente.Nombre = txtNombre.Text.Trim();
            cliente.Apellidos = txtApellidos.Text.Trim();
            cliente.Telefono = txtTelefono.Text.Trim();
            cliente.Email = txtEmail.Text.Trim();
            cliente.FechaAlta = DateTime.Now;

            // Dirección
            cliente.CalleDireccion = txtCalle.Text.Trim();
            cliente.NumeroDireccion = txtNumero.Text.Trim();
            cliente.PisoPuertaDireccion = txtPisoPuerta.Text.Trim();
            cliente.CodigoPostalDireccion = txtCP.Text.Trim();
            cliente.LocalidadDireccion = txtLocalidad.Text.Trim();
            cliente.ProvinciaDireccion = txtProvincia.Text.Trim();

            try
            {
                // Insertar o actualizar según modo
                bool resultado = esEdicion
                    ? clienteService.Actualizar(cliente)
                    : clienteService.Insertar(cliente);

                if (resultado)
                {
                    MessageBox.Show("Cliente guardado correctamente.",
                        "Información",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Cierra ventana y devuelve OK al padre
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el cliente.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se ha producido un error: " + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida todos los campos del formulario
        /// </summary>
        private bool ValidarCampos()
        {
            // DOCUMENTO
            string tipoDoc = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString() ?? "";
            string numeroDoc = txtNumDocumento.Text.Trim();

            if (string.IsNullOrWhiteSpace(numeroDoc))
                return MostrarError("El número de documento es obligatorio.");

            switch (tipoDoc)
            {
                case "DNI":
                    if (!Regex.IsMatch(numeroDoc, @"^\d{8}[A-Z]$"))
                        return MostrarError("El DNI debe tener 8 números y 1 letra.");

                    if (!EsLetraDniValida(numeroDoc))
                        return MostrarError("La letra del DNI no es correcta.");
                    break;

                case "NIE":
                    if (!Regex.IsMatch(numeroDoc, @"^[XYZ]\d{7}[A-Z]$"))
                        return MostrarError("El NIE no tiene un formato válido.");
                    break;
            }

            // DATOS PERSONALES
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                return MostrarError("Los apellidos son obligatorios.");

            if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{9}$"))
                return MostrarError("El teléfono debe contener 9 dígitos.");

            if (!Regex.IsMatch(txtEmail.Text.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return MostrarError("El formato del email no es válido.");

            // DIRECCIÓN
            if (string.IsNullOrWhiteSpace(txtCalle.Text))
                return MostrarError("La calle es obligatoria.");

            if (string.IsNullOrWhiteSpace(txtNumero.Text))
                return MostrarError("El número de la dirección es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtLocalidad.Text))
                return MostrarError("La localidad es obligatoria.");

            if (!Regex.IsMatch(txtCP.Text.Trim(), @"^\d{5}$"))
                return MostrarError("El Código Postal debe tener 5 dígitos.");

            return true;
        }

        /// <summary>
        /// Valida la letra del DNI
        /// </summary>
        private static bool EsLetraDniValida(string documento)
        {
            try
            {
                documento = documento.ToUpper().Trim();

                // Conversión NIE a formato numérico
                string auxiliar = documento;

                if (auxiliar.StartsWith("X")) auxiliar = string.Concat("0", auxiliar.AsSpan(1));
                else if (auxiliar.StartsWith("Y")) auxiliar = string.Concat("1", auxiliar.AsSpan(1));
                else if (auxiliar.StartsWith("Z")) auxiliar = string.Concat("2", auxiliar.AsSpan(1));

                if (!long.TryParse(auxiliar.AsSpan(0, 8), out long numero))
                    return false;

                char letraProporcionada = auxiliar[8];
                string letrasControl = "TRWAGMYFPDXBNJZSQVHLCKE";

                char letraCorrecta = letrasControl[(int)(numero % 23)];

                return letraProporcionada == letraCorrecta;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Muestra mensaje de error de validación
        /// </summary>
        private static bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje,
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return false;
        }

        /// <summary>
        /// Cancela y cierra la ventana
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}