using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Conceptos
{
    /// <summary>
    /// Lógica de interacción para WindowAjustarStock.xaml
    /// </summary>
    public partial class WindowAjustarStock : Window
    {
        // Servicio de lógica de negocio para productos/servicios
        private readonly ConceptoService conceptoService = new();

        // Producto actualmente cargado en la ventana
        private Concepto? producto;

        // Cantidad que se suma o resta al stock original
        private int cantidadAjuste = 0;

        /// <summary>
        /// Constructor que recibe el ID del producto a modificar
        /// </summary>
        public WindowAjustarStock(int idProducto)
        {
            InitializeComponent();

            // Carga inicial del producto en pantalla
            CargarProducto(idProducto);
        }

        /// <summary>
        /// Carga el producto desde base de datos y lo muestra en la UI
        /// </summary>
        private void CargarProducto(int id)
        {
            producto = conceptoService.ObtenerPorId(id);

            if (producto != null)
            {
                // Nombre del producto
                lblNombreProducto.Text = producto.Nombre;

                // Stock actual (si es null, se considera 0)
                txtStockActual.Text = (producto.Stock ?? 0).ToString();

                // Calcula el nuevo stock inicial
                ActualizarCalculo();
            }
        }

        /// <summary>
        /// Recalcula el stock nuevo en función del ajuste
        /// </summary>
        private void ActualizarCalculo()
        {
            if (producto == null) return;

            int stockOriginal = producto.Stock ?? 0;

            // Aplicamos el ajuste
            int nuevoStock = stockOriginal + cantidadAjuste;

            // Evita valores negativos (regla de negocio básica)
            if (nuevoStock < 0)
                nuevoStock = 0;

            txtStockNuevo.Text = nuevoStock.ToString();
        }

        /// <summary>
        /// Incrementa el ajuste de stock (+1)
        /// </summary>
        private void BtnSumar_Click(object sender, RoutedEventArgs e)
        {
            cantidadAjuste++;
            txtCantidad.Text = cantidadAjuste.ToString();
        }

        /// <summary>
        /// Decrementa el ajuste de stock (-1)
        /// </summary>
        private void BtnRestar_Click(object sender, RoutedEventArgs e)
        {
            cantidadAjuste--;
            txtCantidad.Text = cantidadAjuste.ToString();
        }

        /// <summary>
        /// Detecta cambios manuales en el textbox de cantidad
        /// </summary>
        private void TxtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Intenta convertir el texto en número
            if (int.TryParse(txtCantidad.Text, out int result))
            {
                cantidadAjuste = result;
            }
            // Permite estados intermedios como vacío o "-"
            else if (string.IsNullOrEmpty(txtCantidad.Text) || txtCantidad.Text == "-")
            {
                cantidadAjuste = 0;
            }

            // Recalcula el stock
            ActualizarCalculo();
        }

        /// <summary>
        /// Guarda el nuevo stock en la base de datos
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if(producto != null)
            {
                try
                {
                    // Convertimos el resultado final
                    int nuevoStock = int.Parse(txtStockNuevo.Text);

                    // Actualizamos el objeto
                    producto.Stock = nuevoStock;

                    // Persistimos cambios
                    bool exito = conceptoService.Actualizar(producto);

                    if (exito)
                    {
                        // Cierra ventana indicando éxito
                        this.DialogResult = true;
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
        }

        /// <summary>
        /// Cierra la ventana sin guardar cambios
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}