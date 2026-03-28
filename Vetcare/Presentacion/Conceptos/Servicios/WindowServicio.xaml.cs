using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Servicios
{
    public partial class WindowServicio : Window
    {
        private Concepto concepto;
        private ConceptoService conceptoService = new ConceptoService();
        private bool esEdicion = false;

        public WindowServicio()
        {
            InitializeComponent();
            concepto = new Concepto();
            esEdicion = false;
            lblTitulo.Text = "NUEVO SERVICIO";
        }

        public WindowServicio(Concepto servicioExistente)
        {
            InitializeComponent();
            concepto = servicioExistente;
            esEdicion = true;
            lblTitulo.Text = "EDITAR SERVICIO";
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = concepto.Nombre;
            txtPrecio.Text = concepto.Precio.ToString("N2");
            txtDescripcion.Text = concepto.Descripcion;

            // --- LÓGICA PARA SELECCIONAR EL IVA EN EL COMBOBOX ---
            string ivaGuardado = concepto.IvaPorcentaje.ToString("G29"); // Quita ceros innecesarios
            foreach (ComboBoxItem item in cmbIva.Items)
            {
                if (item.Content.ToString() == ivaGuardado)
                {
                    cmbIva.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                // Mapeo básico
                concepto.Nombre = txtNombre.Text.Trim();
                concepto.Precio = decimal.Parse(txtPrecio.Text.Trim());
                concepto.Descripcion = txtDescripcion.Text.Trim();
                concepto.Tipo = "Servicio";
                concepto.Activo = true;

                // --- OBTENER VALOR DEL COMBOBOX ---
                if (cmbIva.SelectedItem is ComboBoxItem selectedItem)
                {
                    concepto.IvaPorcentaje = decimal.Parse(selectedItem.Content.ToString());
                }

                bool exito = esEdicion ?
                    conceptoService.Actualizar(concepto) :
                    conceptoService.Insertar(concepto);

                if (exito)
                {
                    MessageBox.Show("Servicio guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar el servicio: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out _))
            {
                MessageBox.Show("Ingrese un precio válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        // Mantenemos solo la restricción de entrada para el precio
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isNumber = System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9,]");
            if (!isNumber || (e.Text == "," && ((TextBox)sender).Text.Contains(",")))
            {
                e.Handled = true;
            }
        }
    }
}