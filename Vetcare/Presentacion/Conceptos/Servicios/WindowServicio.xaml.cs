using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Ventana para crear o editar un servicio dentro del sistema.
    /// </summary>
    public partial class WindowServicio : Window
    {
        // Objeto servicio que se está creando o editando
        private readonly Concepto concepto;

        // Servicio de acceso a datos
        private readonly ConceptoService conceptoService = new();

        // Indica si estamos en modo edición o creación
        private readonly bool esEdicion = false;

        [GeneratedRegex("[0-9,]")]
        private static partial Regex PrecioRegex();


        /// <summary>
        /// Constructor para crear un nuevo servicio.
        /// </summary>
        public WindowServicio()
        {
            InitializeComponent();

            concepto = new Concepto();
            esEdicion = false;

            // Configuración de la UI
            lblTitulo.Text = "NUEVO SERVICIO";
        }

        /// <summary>
        /// Constructor para editar un servicio existente.
        /// </summary>
        /// <param name="servicioExistente">Servicio a editar</param>
        public WindowServicio(Concepto servicioExistente)
        {
            InitializeComponent();

            concepto = servicioExistente;
            esEdicion = true;

            // Configuración de la UI
            lblTitulo.Text = "EDITAR SERVICIO";

            // Carga los datos existentes en los controles
            CargarDatos();
        }

        /// <summary>
        /// Carga los datos del servicio en los controles de la interfaz.
        /// </summary>
        private void CargarDatos()
        {
            txtNombre.Text = concepto.Nombre;
            txtPrecio.Text = concepto.Precio.ToString("N2");
            txtDescripcion.Text = concepto.Descripcion;

            // Seleccionar el IVA correspondiente en el ComboBox
            string ivaGuardado = concepto.IvaPorcentaje.ToString("G29");

            foreach (ComboBoxItem item in cmbIva.Items)
            {
                if (item.Content.ToString() == ivaGuardado)
                {
                    cmbIva.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Evento que se ejecuta al pulsar el botón Guardar.
        /// Inserta o actualiza el servicio en la base de datos.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validar datos antes de continuar
            if (!Validar()) return;

            try
            {
                // Mapeo de datos desde la UI al objeto
                concepto.Nombre = txtNombre.Text.Trim();
                concepto.Precio = decimal.Parse(txtPrecio.Text.Trim());
                concepto.Descripcion = txtDescripcion.Text.Trim();
                concepto.Tipo = "Servicio";
                concepto.Activo = true;

                // Obtener valor seleccionado del ComboBox de IVA
                if (cmbIva.SelectedItem is ComboBoxItem selectedItem)
                {
                    concepto.IvaPorcentaje = decimal.Parse(selectedItem.Content.ToString() ?? "");
                }

                // Insertar o actualizar según el modo
                bool exito = esEdicion
                    ? conceptoService.Actualizar(concepto)
                    : conceptoService.Insertar(concepto);

                if (exito)
                {
                    MessageBox.Show("Servicio guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Cierra la ventana indicando éxito
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                MessageBox.Show("Error al procesar el servicio: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida los datos introducidos por el usuario.
        /// </summary>
        /// <returns>True si los datos son válidos, false en caso contrario</returns>
        private bool Validar()
        {
            // Validar nombre obligatorio
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validar formato del precio
            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("Ingrese un precio válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cierra la ventana sin guardar cambios.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        /// <summary>
        /// Permite únicamente números y coma en el campo de precio.
        /// </summary>
        private void TxtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumber = PrecioRegex().IsMatch(e.Text);

            if (!isNumber || (e.Text == "," && ((TextBox)sender).Text.Contains(',')))
            {
                e.Handled = true;
            }
        }
    }
}