using System;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion
{
    public partial class WindowVeterinario : Window
    {
        private Veterinario veterinario;
        private VeterinarioService veterinarioService = new VeterinarioService();
        private bool esEdicion = false;

        // NUEVO
        public WindowVeterinario()
        {
            InitializeComponent();

            veterinario = new Veterinario();
            esEdicion = false;

            lblTitulo.Text = "NUEVO VETERINARIO";
            this.Title = "Nuevo Veterinario";
        }

        // EDITAR
        public WindowVeterinario(Veterinario veterinarioExistente)
        {
            InitializeComponent();

            veterinario = veterinarioExistente;
            esEdicion = true;

            lblTitulo.Text = "EDITAR VETERINARIO";
            this.Title = "Editar Veterinario";

            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = veterinario.Nombre;
            txtApellidos.Text = veterinario.Apellidos;
            txtEspecialidad.Text = veterinario.Especialidad;
            txtNumeroColegiado.Text = veterinario.NumeroColegiado;
            txtTelefono.Text = veterinario.Telefono;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                veterinario.Nombre = txtNombre.Text.Trim();
                veterinario.Apellidos = txtApellidos.Text.Trim();
                veterinario.Especialidad = txtEspecialidad.Text.Trim();
                veterinario.NumeroColegiado = txtNumeroColegiado.Text.Trim();
                veterinario.Telefono = txtTelefono.Text.Trim();

                bool resultado;

                if (esEdicion)
                    resultado = veterinarioService.Actualizar(veterinario);
                else
                    resultado = veterinarioService.Insertar(veterinario);

                if (resultado)
                {
                    MessageBox.Show("Veterinario guardado correctamente.",
                                    "Éxito",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el veterinario.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inesperado: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                return MostrarError("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtApellidos.Text))
                return MostrarError("Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(txtNumeroColegiado.Text))
                return MostrarError("El número de colegiado es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                return MostrarError("La especialidad es obligatoria.");

            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje,
                            "Validación",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
            return false;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}