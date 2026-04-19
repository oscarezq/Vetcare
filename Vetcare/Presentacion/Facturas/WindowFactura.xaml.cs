using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Clientes;

namespace Vetcare.Presentacion.Facturas
{
    /// <summary>
    /// Ventana principal para la creación y gestión de facturas
    /// </summary>
    public partial class WindowFactura : Window
    {
        // Colección observable que almacena las líneas de la factura
        private readonly ObservableCollection<DetalleFactura> listaDetalles = new();

        // Servicio de lógica de negocio para facturas
        private readonly FacturaService facturaService = new();

        // Servicio de lógica de negocio para conceptos
        private readonly ConceptoService conceptoService = new();

        // Concepto seleccionado (producto o servicio)
        private Concepto? conceptoSeleccionado;

        /// <summary>
        /// Constructor principal
        /// </summary>
        public WindowFactura()
        {
            InitializeComponent();

            // Vinculamos el DataGrid con la colección
            dgDetalles.ItemsSource = listaDetalles;

            // Generamos el siguiente número de factura
            txtNumFactura.Text = ObtenerSiguienteNumeroFactura();
        }

        /// <summary>
        /// Constructor con cliente preseleccionado
        /// </summary>
        public WindowFactura(Cliente cliente) : this()
        {
            if (cliente != null)
            {
                // Asignar datos a los campos de texto
                txtIdCliente.Text = cliente.IdCliente.ToString();
                txtNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellidos}";

                // Estilo visual de "seleccionado"
                txtNombreCliente.Foreground = Brushes.Black;
                txtNombreCliente.FontWeight = FontWeights.Bold;

                // Bloquear modificación
                btnSeleccionarCliente.IsEnabled = false;
                btnSeleccionarCliente.ToolTip = "El cliente no se puede modificar para esta operación.";
            }
        }

        /// <summary>
        /// Obtiene el siguiente número de factura basado en el año actual
        /// </summary>
        public string ObtenerSiguienteNumeroFactura()
        {
            int anioActual = DateTime.Now.Year;

            // Obtiene el último número registrado en el sistema
            string ultimoNumero = facturaService.ObtenerUltimoNumeroPorAnio(anioActual) ?? "";

            // Si no existe ninguno, empezamos en 0001
            if (string.IsNullOrEmpty(ultimoNumero))
                return $"{anioActual}-0001";

            // Separa año y contador
            string[] partes = ultimoNumero.Split('-');

            if (partes.Length == 2 && int.TryParse(partes[1], out int contador))
            {
                contador++;
                return $"{anioActual}-{contador:D4}";
            }

            return $"{anioActual}-0001";
        }

        /// <summary>
        /// Abre la ventana para seleccionar cliente
        /// </summary>
        private void BtnSeleccionarCliente_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorCliente selector = new() { 
                Owner = Window.GetWindow(this)
            };

            if (selector.ShowDialog() == true)
            {
                var cliente = selector.ClienteSeleccionado;

                if(cliente != null ) {
                    txtIdCliente.Text = cliente.IdCliente.ToString();
                    txtNombreCliente.Text = $"{cliente.Nombre} {cliente.Apellidos}";

                    // Estilo visual
                    txtNombreCliente.Foreground = Brushes.Black;
                    txtNombreCliente.FontWeight = FontWeights.Bold;
                }
            }
        }

        /// <summary>
        /// Abre el selector de productos/servicios
        /// </summary>
        private void BtnSeleccionarServicio_Click(object sender, RoutedEventArgs e)
        {
            WindowSelectorConcepto selector = new () { 
                Owner = Window.GetWindow(this)
            };

            if (selector.ShowDialog() == true)
            {
                conceptoSeleccionado = selector.ConceptoSeleccionado;

                if(conceptoSeleccionado != null)
                {
                    txtNombreServicio.Text = $"{conceptoSeleccionado.Nombre} ({conceptoSeleccionado.Precio:N2}€)";

                    // Estilo visual
                    txtNombreServicio.Foreground = Brushes.Black;
                    txtNombreServicio.FontWeight = FontWeights.Bold;
                }
            }
        }

        /// <summary>
        /// Añade un concepto a la factura
        /// </summary>
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Validación: debe haber concepto seleccionado
            if (conceptoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto o servicio.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener cantidad
            _ = int.TryParse(txtCantidad.Text, out int cantidad);
            if (cantidad <= 0) 
                cantidad = 1;

            // Si el concepto es producto, validar si hay stock suficiente
            if (conceptoSeleccionado.Tipo == "Producto" && conceptoSeleccionado.Stock < cantidad)
            {
                MessageBox.Show($"Stock insuficiente. Stock disponible: {conceptoSeleccionado.Stock}",
                                "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Buscar si ya existe el concepto en la lista
            var existente = listaDetalles.FirstOrDefault(d =>
                d.IdConcepto == conceptoSeleccionado.IdConcepto &&
                d.Tipo == conceptoSeleccionado.Tipo);

            if (existente != null)
            {
                // Si existe, sumar cantidad
                existente.Cantidad += cantidad;
            }
            else
            {
                // Si no existe, crear nueva línea
                listaDetalles.Add(new DetalleFactura
                {
                    IdConcepto = conceptoSeleccionado.IdConcepto,
                    NombreConcepto = conceptoSeleccionado.Nombre,
                    Tipo = conceptoSeleccionado.Tipo,
                    Cantidad = cantidad,
                    PrecioUnitario = conceptoSeleccionado.Precio,
                    IvaPorcentaje = conceptoSeleccionado.IvaPorcentaje
                });
            }

            // Actualizar interfaz
            ActualizarInterfaz();

            // Limpiar selección
            LimpiarSelectorConcepto();
        }

        /// <summary>
        /// Elimina una línea de la factura
        /// </summary>
        private void BtnQuitar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: DetalleFactura detalle })
            {
                listaDetalles.Remove(detalle);
                ActualizarInterfaz();
            }
        }

        /// <summary>
        /// Disminuye la cantidad de una línea
        /// </summary>
        private void BtnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: DetalleFactura detalle } && detalle.Cantidad > 1)
            {
                detalle.Cantidad--;
                ActualizarInterfaz();
            }
        }

        /// <summary>
        /// Aumenta la cantidad de una línea
        /// </summary>
        private void BtnAumentar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: DetalleFactura detalle })
            {
                // Servicios no tienen límite de stock
                if (detalle.Tipo == "Servicio")
                {
                    detalle.Cantidad++;
                    ActualizarInterfaz();
                }
                else if (detalle.Tipo == "Producto")
                {
                    // Validar stock real desde servicio
                    var concepto = conceptoService.ObtenerPorId(detalle.IdConcepto);

                    if (concepto != null)
                    {
                        if (detalle.Cantidad + 1 <= concepto.Stock)
                        {
                            detalle.Cantidad++;
                            ActualizarInterfaz();
                        }
                        else
                        {
                            MessageBox.Show($"No hay más stock disponible para {detalle.NombreConcepto}. " +
                                            $"Stock máximo: {concepto.Stock}",
                                            "Stock insuficiente",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Warning);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Guarda la factura en el sistema
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validación de cliente
            if (string.IsNullOrEmpty(txtIdCliente.Text))
            {
                MessageBox.Show("Por favor, seleccione un cliente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validación de conceptos
            if (listaDetalles.Count == 0)
            {
                MessageBox.Show("No se puede crear una factura sin conceptos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmación del usuario
            if (MessageBox.Show("¿Generar factura?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Recalcular antes de guardar
                    ActualizarInterfaz();

                    // Cálculos
                    decimal baseImp = listaDetalles.Sum(d => d.Subtotal);
                    decimal ivaTotal = listaDetalles.Sum(d => d.IvaImporte);
                    decimal total = listaDetalles.Sum(d => d.TotalLinea);

                    // Crear objeto factura
                    Factura nuevaFactura = new()
                    {
                        NumeroFactura = txtNumFactura.Text,
                        IdCliente = int.Parse(txtIdCliente.Text),
                        MetodoPago = (cbMetodoPago.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Efectivo",
                        Observaciones = txtObservaciones.Text,
                        BaseImponible = Math.Round(baseImp, 2),
                        IvaTotal = Math.Round(ivaTotal, 2),
                        Total = Math.Round(total, 2),
                        Detalles = listaDetalles.ToList()
                    };

                    // Guardar en base de datos
                    if (facturaService.InsertarFacturaCompleta(nuevaFactura))
                    {
                        MessageBox.Show("Factura guardada correctamente.", "Éxito");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo guardar: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Actualiza la UI y recalcula totales
        /// </summary>
        private void ActualizarInterfaz()
        {
            // Forzar fin de edición en DataGrid
            dgDetalles.CommitEdit(DataGridEditingUnit.Row, true);
            dgDetalles.CancelEdit();

            // Refrescar datos
            dgDetalles.Items.Refresh();

            // Recalcular totales
            decimal baseImp = listaDetalles.Sum(d => d.Subtotal);
            decimal ivaTotal = listaDetalles.Sum(d => d.IvaImporte);
            decimal total = listaDetalles.Sum(d => d.TotalLinea);

            // Mostrar resultados
            lblBaseImponible.Text = $"{baseImp:N2} €";
            lblIvaTotal.Text = $"{ivaTotal:N2} €";
            lblTotal.Text = $"{total:N2} €";
        }

        /// <summary>
        /// Limpia el selector de concepto
        /// </summary>
        private void LimpiarSelectorConcepto()
        {
            conceptoSeleccionado = null;

            txtNombreServicio.Text = "Seleccionar concepto...";
            txtNombreServicio.Foreground = Brushes.Gray;
            txtNombreServicio.FontWeight = FontWeights.Normal;

            txtCantidad.Text = "1";
        }

        /// <summary>
        /// Incrementa cantidad desde input
        /// </summary>
        private void BtnCantidadMas_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int c))
                txtCantidad.Text = (c + 1).ToString();
        }

        /// <summary>
        /// Disminuye cantidad desde input
        /// </summary>
        private void BtnCantidadMenos_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int c) && c > 1)
                txtCantidad.Text = (c - 1).ToString();
        }

        /// <summary>
        /// Cierra la ventana sin guardar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}