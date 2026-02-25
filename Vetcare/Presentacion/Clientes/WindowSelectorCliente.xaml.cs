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
    public partial class WindowSelectorCliente : Window
    {
        // Lista completa para filtrar en memoria sin ir a la DB cada vez
        private List<Cliente> listaClientes;
        public Cliente ClienteSeleccionado { get; set; }

        public WindowSelectorCliente()
        {
            InitializeComponent();
            CargarLista();
        }

        private void CargarLista()
        {
            // Cargamos todos los clientes al abrir la ventana
            listaClientes = new ClienteService().ObtenerTodos();
            dgClientes.ItemsSource = listaClientes;
        }

        private void txtBuscaCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaClientes == null) return;

            string busqueda = txtBuscaCliente.Text.ToLower().Trim();

            // Filtro inteligente por múltiples campos
            var filtrados = listaClientes.Where(c =>
                (c.Nombre != null && c.Nombre.ToLower().Contains(busqueda)) ||
                (c.Apellidos != null && c.Apellidos.ToLower().Contains(busqueda)) ||
                (c.NumDocumento != null && c.NumDocumento.ToLower().Contains(busqueda))
            ).ToList();

            dgClientes.ItemsSource = filtrados;
        }

        private void FinalizarSeleccion()
        {
            if (dgClientes.SelectedItem != null)
            {
                ClienteSeleccionado = (Cliente)dgClientes.SelectedItem;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Seleccione un cliente de la lista", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FinalizarSeleccion();
        }

        private void btnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            FinalizarSeleccion();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
