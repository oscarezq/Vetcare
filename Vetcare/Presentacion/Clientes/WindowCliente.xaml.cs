using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    public partial class WindowCliente : Window
    {
        private Cliente cliente;
        private ClienteService clienteService;
        private bool esEdicion = false;

        public WindowCliente()
        {
            InitializeComponent();
            clienteService = new ClienteService();

            cliente = new Cliente
            {
                FechaAlta = DateTime.Now
            };

            lblTitulo.Text = "NUEVO CLIENTE";
            this.Title = "Nuevo cliente";
            cmbTipoDocumento.SelectedIndex = 0;
        }

        public WindowCliente(Cliente clienteExistente)
        {
            InitializeComponent();
            clienteService = new ClienteService();
            cliente = clienteExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR CLIENTE";
            this.Title = "Editar cliente";

            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNumDocumento.Text = cliente.NumDocumento;
            txtNombre.Text = cliente.Nombre;
            txtApellidos.Text = cliente.Apellidos;
            txtTelefono.Text = cliente.Telefono;
            txtEmail.Text = cliente.Email;

            // Nuevos campos de dirección
            txtCalle.Text = cliente.CalleDireccion;
            txtNumero.Text = cliente.NumeroDireccion;
            txtPisoPuerta.Text = cliente.PisoPuertaDireccion;
            txtCP.Text = cliente.CodigoPostalDireccion;
            txtLocalidad.Text = cliente.LocalidadDireccion;
            txtProvincia.Text = cliente.ProvinciaDireccion;

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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos())
                return;

            // Asignación de datos básicos
            cliente.TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();
            cliente.NumDocumento = txtNumDocumento.Text.Trim().ToUpper();
            cliente.Nombre = txtNombre.Text.Trim();
            cliente.Apellidos = txtApellidos.Text.Trim();
            cliente.Telefono = txtTelefono.Text.Trim();
            cliente.Email = txtEmail.Text.Trim();

            // Asignación de nuevos campos de dirección
            cliente.CalleDireccion = txtCalle.Text.Trim();
            cliente.NumeroDireccion = txtNumero.Text.Trim();
            cliente.PisoPuertaDireccion = txtPisoPuerta.Text.Trim();
            cliente.CodigoPostalDireccion = txtCP.Text.Trim();
            cliente.LocalidadDireccion = txtLocalidad.Text.Trim();
            cliente.ProvinciaDireccion = txtProvincia.Text.Trim();

            try
            {
                bool resultado = esEdicion ? clienteService.Actualizar(cliente) : clienteService.Insertar(cliente);

                if (resultado)
                {
                    MessageBox.Show("Cliente guardado correctamente.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se ha producido un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            // 1. Validación de Documento
            string tipoDoc = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();
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

            // 2. Validación de Datos Personales
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return MostrarError("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtApellidos.Text)) return MostrarError("Los apellidos son obligatorios.");
            
            if (!Regex.IsMatch(txtTelefono.Text.Trim(), @"^\d{9}$"))
                return MostrarError("El teléfono debe contener 9 dígitos.");

            if (!Regex.IsMatch(txtEmail.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return MostrarError("El formato del email no es válido.");

            // 3. Validación de Dirección (Campos nuevos)
            if (string.IsNullOrWhiteSpace(txtCalle.Text)) return MostrarError("La calle es obligatoria.");
            if (string.IsNullOrWhiteSpace(txtNumero.Text)) return MostrarError("El número de la dirección es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtLocalidad.Text)) return MostrarError("La localidad es obligatoria.");
            
            if (!Regex.IsMatch(txtCP.Text.Trim(), @"^\d{5}$"))
                return MostrarError("El Código Postal debe tener 5 dígitos.");

            return true;
        }

        private bool EsLetraDniValida(string documento)
        {
            try
            {
                documento = documento.ToUpper().Trim();
                string auxiliar = documento;
                if (auxiliar.StartsWith("X")) auxiliar = "0" + auxiliar.Substring(1);
                else if (auxiliar.StartsWith("Y")) auxiliar = "1" + auxiliar.Substring(1);
                else if (auxiliar.StartsWith("Z")) auxiliar = "2" + auxiliar.Substring(1);

                if (!long.TryParse(auxiliar.Substring(0, 8), out long numero)) return false;
                char letraProporcionada = auxiliar[8];
                string letrasControl = "TRWAGMYFPDXBNJZSQVHLCKE";
                char letraCorrecta = letrasControl[(int)(numero % 23)];

                return letraProporcionada == letraCorrecta;
            }
            catch { return false; }
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
    }
}