using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Servicios
{
    /// <summary>
    /// Lógica de interacción para WindowServicio.xaml
    /// </summary>
    public partial class WindowServicio : Window
    {
        private Concepto concepto;
        private ConceptoService conceptoService = new ConceptoService(); // Asumiendo que tienes esta capa
        private bool esEdicion = false;

        // Constructor para Nuevo
        public WindowServicio()
        {
            InitializeComponent();
            concepto = new Concepto();
            esEdicion = false;
            lblTitulo.Text = "NUEVO SERVICIO";
        }

        // Constructor para Editar
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
            txtPrecio.Text = concepto.PrecioBase.ToString("N2");
            txtIva.Text = concepto.IvaPorcentaje.ToString("N2");
            txtDescripcion.Text = concepto.Descripcion;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                // Mapeo
                concepto.Nombre = txtNombre.Text.Trim();
                concepto.PrecioBase = decimal.Parse(txtPrecio.Text.Trim());
                concepto.IvaPorcentaje = decimal.Parse(txtIva.Text.Trim());
                concepto.Descripcion = txtDescripcion.Text.Trim();
                concepto.Tipo = "Servicio";
                concepto.Activo = true;

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
                MessageBox.Show("Error al procesar el servicio: " + ex.Message);
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
                MessageBox.Show("Ingrese un precio válido (use coma para decimales según su región).", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtIva.Text, out decimal ivaValue) || ivaValue < 0 || ivaValue > 100)
            {
                MessageBox.Show("Ingrese un IVA válido entre 0 y 100.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        // 1. Solo permite números y una coma decimal
        private void txtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permite números y comprueba si ya existe una coma
            bool isNumber = System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9,]");

            if (!isNumber || (e.Text == "," && ((TextBox)sender).Text.Contains(",")))
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        // 2. Autocorrección del IVA al salir del cuadro de texto
        private void txtIva_LostFocus(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor))
            {
                if (valor > 100) txtIva.Text = "100";
                if (valor < 0) txtIva.Text = "0";
            }
            else
            {
                txtIva.Text = "0"; // Si borran todo y salen, ponemos 0 por defecto
            }
        }

        // 3. Los métodos de los botones (se mantienen igual pero ahora RepeatButton los llama rápido)
        private void btnSubirIva_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor) && valor < 100)
            {
                txtIva.Text = (valor + 1).ToString();
            }
        }

        private void btnBajarIva_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtIva.Text, out decimal valor) && valor > 0)
            {
                txtIva.Text = (valor - 1).ToString();
            }
        }
    }
}
