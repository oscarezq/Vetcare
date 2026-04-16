using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Clientes
{
    /// <summary>
    /// Ventana para seleccionar un cliente existente.
    /// Permite buscar, filtrar y crear nuevos clientes.
    /// </summary>
    public partial class WindowSelectorCliente : Window
    {
        // Lista completa de clientes activos
        private List<Cliente>? listaClientes;

        // Cliente seleccionado que se devolverá a la ventana llamadora
        public Cliente? ClienteSeleccionado { get; set; }

        /// <summary>
        /// Constructor de la ventana.
        /// Carga la lista de clientes al iniciar.
        /// </summary>
        public WindowSelectorCliente()
        {
            InitializeComponent();
            CargarLista();
        }

        /// <summary>
        /// Carga todos los clientes activos desde la base de datos
        /// y los asigna al DataGrid.
        /// </summary>
        private void CargarLista()
        {
            // Obtenemos clientes activos
            listaClientes = new ClienteService().ObtenerTodos()
                                    .Where(m => m.Activo == true)
                                    .ToList();

            // Asignamos al DataGrid
            dgClientes.ItemsSource = listaClientes;
        }

        /// <summary>
        /// Evento que se dispara al escribir en el buscador.
        /// Filtra la lista en memoria por nombre, apellidos o documento.
        /// </summary>
        private void TxtBuscaCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaClientes == null) return;

            // Texto de búsqueda en minúsculas
            string busqueda = txtBuscaCliente.Text.ToLower().Trim();

            // Filtro por múltiples campos
            var filtrados = listaClientes.Where(c =>
                (c.Nombre != null && c.Nombre.ToLower().Contains(busqueda)) ||
                (c.Apellidos != null && c.Apellidos.ToLower().Contains(busqueda)) ||
                (c.NumDocumento != null && c.NumDocumento.ToLower().Contains(busqueda))
            ).ToList();

            // Actualizamos el DataGrid con los resultados filtrados
            dgClientes.ItemsSource = filtrados;
        }

        /// <summary>
        /// Finaliza la selección del cliente.
        /// Devuelve el cliente seleccionado y cierra la ventana.
        /// </summary>
        private void FinalizarSeleccion()
        {
            if (dgClientes.SelectedItem != null)
            {
                // Guardamos el cliente seleccionado
                ClienteSeleccionado = (Cliente)dgClientes.SelectedItem;

                // Indicamos que la operación fue correcta
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Aviso si no hay selección
                MessageBox.Show("Seleccione un cliente de la lista", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Evento doble clic en el DataGrid.
        /// Permite seleccionar rápidamente un cliente.
        /// </summary>
        private void DgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();

        /// <summary>
        /// Evento click del botón "Seleccionar".
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            FinalizarSeleccion();
        }

        /// <summary>
        /// Evento click del botón "Cancelar".
        /// Cierra la ventana sin seleccionar cliente.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Evento click del botón "Nuevo Cliente".
        /// Abre la ventana de creación de cliente y refresca la lista.
        /// </summary>
        private void BtnNuevoCliente_Click(object sender, RoutedEventArgs e)
        {
            // Creamos la ventana de nuevo cliente
            WindowCliente ventana = new()
            {
                Owner = Window.GetWindow(this)
            };

            // Si se crea correctamente
            if (ventana.ShowDialog() == true)
            {
                // Refresca el DataGrid para mostrar el nuevo cliente
                CargarLista();

                // Selecciona automáticamente el último cliente añadido
                if (dgClientes.Items.Count > 0)
                {
                    dgClientes.SelectedIndex = dgClientes.Items.Count - 1;
                }
            }
        }
    }
}