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
        private Servicio servicio;
        private ServicioService servicioService = new ServicioService(); // Asumiendo que tienes esta capa
        private bool esEdicion = false;

        // Constructor para Nuevo
        public WindowServicio()
        {
            InitializeComponent();
            servicio = new Servicio();
            esEdicion = false;
            lblTitulo.Text = "NUEVO SERVICIO";
        }

        // Constructor para Editar
        public WindowServicio(Servicio servicioExistente)
        {
            InitializeComponent();
            servicio = servicioExistente;
            esEdicion = true;
            lblTitulo.Text = "EDITAR SERVICIO";
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = servicio.Nombre;
            txtPrecio.Text = servicio.PrecioBase.ToString();
            txtDescripcion.Text = servicio.Descripcion;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            try
            {
                // Mapeo
                servicio.Nombre = txtNombre.Text.Trim();
                servicio.PrecioBase = decimal.Parse(txtPrecio.Text.Trim());
                servicio.Descripcion = txtDescripcion.Text.Trim();

                bool exito = esEdicion ?
                    servicioService.Actualizar(servicio) :
                    servicioService.Insertar(servicio);

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

            return true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}
