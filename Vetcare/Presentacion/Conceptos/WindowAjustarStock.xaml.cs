using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Conceptos
{
    public partial class WindowAjustarStock : Window
    {
        private ConceptoService conceptoService = new ConceptoService();
        private Concepto producto;
        private int cantidadAjuste = 0;

        public WindowAjustarStock(int idProducto)
        {
            InitializeComponent();
            CargarProducto(idProducto);
        }

        private void CargarProducto(int id)
        {
            producto = conceptoService.ObtenerPorId(id);
            if (producto != null)
            {
                lblNombreProducto.Text = producto.Nombre;
                txtStockActual.Text = (producto.Stock ?? 0).ToString();
                ActualizarCalculo();
            }
        }

        private void ActualizarCalculo()
        {
            if (producto == null) return;

            int stockOriginal = producto.Stock ?? 0;
            int nuevoStock = stockOriginal + cantidadAjuste;

            // Evitar stock negativo (opcional, según tu negocio)
            if (nuevoStock < 0) nuevoStock = 0;

            txtStockNuevo.Text = nuevoStock.ToString();
        }

        private void btnSumar_Click(object sender, RoutedEventArgs e)
        {
            cantidadAjuste++;
            txtCantidad.Text = cantidadAjuste.ToString();
        }

        private void btnRestar_Click(object sender, RoutedEventArgs e)
        {
            cantidadAjuste--;
            txtCantidad.Text = cantidadAjuste.ToString();
        }

        private void txtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int result))
            {
                cantidadAjuste = result;
            }
            else if (string.IsNullOrEmpty(txtCantidad.Text) || txtCantidad.Text == "-")
            {
                cantidadAjuste = 0;
            }

            ActualizarCalculo();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nuevoStock = int.Parse(txtStockNuevo.Text);

                // Actualizamos el objeto y enviamos a la base de datos
                producto.Stock = nuevoStock;
                bool exito = conceptoService.Actualizar(producto);

                if (exito)
                {
                    this.DialogResult = true; // Esto cierra la ventana y avisa éxito
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el stock en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}