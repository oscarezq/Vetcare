using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;

namespace Vetcare.Presentacion.Usuarios
{
    public partial class WindowVerPerfil : Window
    {
        private Usuario _usuario;
        private UsuarioService _usuarioService = new UsuarioService();
        private VeterinarioService _veteService = new VeterinarioService();

        public WindowVerPerfil(Usuario usuario)
        {
            InitializeComponent();
            this._usuario = usuario;
            CargarDatos();
        }

        private void CargarDatos()
        {
            // Header y Cuenta
            txtNombreHeader.Text = $"{_usuario.Nombre} {_usuario.Apellidos}".ToUpper();
            txtRolHeader.Text = _usuario.NombreRol;
            txtUsername.Text = _usuario.Username;
            txtFechaAlta.Text = _usuario.FechaAlta.ToString("dd/MM/yyyy HH:mm");

            // Datos Personales
            txtNombre.Text = _usuario.Nombre;
            txtApellidos.Text = _usuario.Apellidos;
            txtEmail.Text = _usuario.Email;
            txtTelefono.Text = _usuario.Telefono;

            // Lógica para Veterinario (Basada en tu método de validación)
            if (EsRolVeterinario())
            {
                borderVeterinario.Visibility = Visibility.Visible;

                // Intentamos traer los datos específicos de la tabla veterinarios
                var datosPro = _veteService.ObtenerPorIdUsuario(_usuario.IdUsuario);
                if (datosPro != null)
                {
                    txtEspecialidad.Text = datosPro.Especialidad;
                    txtColegiado.Text = datosPro.NumeroColegiado;
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Ejecutar validaciones antes de procesar
            if (!ValidarCampos()) return;

            try
            {
                // 2. Mapear datos a actualizar
                _usuario.Nombre = txtNombre.Text.Trim();
                _usuario.Apellidos = txtApellidos.Text.Trim();
                _usuario.Email = txtEmail.Text.Trim();
                _usuario.Telefono = txtTelefono.Text.Trim();

                // 3. Guardar en tabla Usuarios
                if (_usuarioService.Actualizar(_usuario))
                {
                    // 4. Si es veterinario, guardar datos profesionales
                    if (EsRolVeterinario())
                    {
                        GuardarDatosProfesionales(_usuario.IdUsuario);
                    }

                    MessageBox.Show("Perfil actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar la información básica.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GuardarDatosProfesionales(int idUsuario)
        {
            Veterinario vete = new Veterinario
            {
                IdUsuario = idUsuario,
                Especialidad = txtEspecialidad.Text.Trim(),
                NumeroColegiado = txtColegiado.Text.Trim()
            };

            // Intentamos actualizar, si no existe (porque el rol cambió a vete después), insertamos
            if (!_veteService.Actualizar(vete))
            {
                _veteService.Insertar(vete);
            }
        }

        private bool EsRolVeterinario()
        {
            return !string.IsNullOrEmpty(_usuario.NombreRol) &&
                   _usuario.NombreRol.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase);
        }

        private bool ValidarCampos()
        {
            // Obligatorios
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return MostrarError("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtApellidos.Text)) return MostrarError("Los apellidos son obligatorios.");
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) return MostrarError("El email es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtTelefono.Text)) return MostrarError("El teléfono es obligatorio.");

            // Formatos (Regex)
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return MostrarError("El formato del email no es válido.");

            if (!Regex.IsMatch(txtTelefono.Text, @"^[0-9]{9}$"))
                return MostrarError("El teléfono debe tener 9 dígitos numéricos.");

            // Específicos de Veterinario
            if (EsRolVeterinario())
            {
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                    return MostrarError("La especialidad es obligatoria.");

                if (!Regex.IsMatch(txtColegiado.Text, @"^[0-9]{9}$"))
                    return MostrarError("El número de colegiado debe tener 9 dígitos numéricos.");
            }

            return true;
        }

        private bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}