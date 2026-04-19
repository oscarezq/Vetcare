using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;

namespace Vetcare.Presentacion.Usuarios
{
    /// <summary>
    /// Ventana de visualización y edición del perfil del usuario
    /// </summary>
    public partial class WindowVerPerfil : Window
    {
        // Usuario actualmente cargado en el perfil
        private readonly Usuario _usuario;

        // Servicio para operaciones sobre usuarios
        private readonly UsuarioService _usuarioService = new();

        // Servicio para datos específicos de veterinario
        private readonly VeterinarioService _veteService = new();

        [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")]
        private static partial Regex RegexEmail();

        [GeneratedRegex("^[0-9]{9}$")]
        private static partial Regex RegexTelefono();

        [GeneratedRegex("^[0-9]{9}$")]
        private static partial Regex RegexNumColegiado();

        /// <summary>
        /// Constructor de la ventana de perfil
        /// </summary>
        /// <param name="usuario">Usuario que se va a visualizar</param>
        public WindowVerPerfil(Usuario usuario)
        {
            InitializeComponent();

            this._usuario = usuario;

            // Cargar datos del usuario
            CargarDatos();
        }

        /// <summary>
        /// Carga todos los datos del usuario en la interfaz
        /// </summary>
        private void CargarDatos()
        {
            // Header y datos principales del usuario
            txtNombreHeader.Text = $"{_usuario.Nombre} {_usuario.Apellidos}".ToUpper();
            txtRolHeader.Text = _usuario.NombreRol;
            txtUsername.Text = _usuario.Username;
            txtFechaAlta.Text = _usuario.FechaAlta.ToString("dd/MM/yyyy HH:mm");

            // Datos personales
            txtNombre.Text = _usuario.Nombre;
            txtApellidos.Text = _usuario.Apellidos;
            txtEmail.Text = _usuario.Email;
            txtTelefono.Text = _usuario.Telefono;

            // Comprobamos si el usuario es veterinario
            if (EsRolVeterinario())
            {
                // Mostramos sección profesional
                borderVeterinario.Visibility = Visibility.Visible;

                // Obtenemos datos profesionales del veterinario
                var datosPro = _veteService.ObtenerPorIdUsuario(_usuario.IdUsuario);
                if (datosPro != null)
                {
                    txtEspecialidad.Text = datosPro.Especialidad;
                    txtColegiado.Text = datosPro.NumeroColegiado;
                }
            }
        }

        /// <summary>
        /// Evento al guardar cambios del perfil
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validamos antes de guardar
            if (!ValidarCampos()) return;

            try
            {
                // Actualizamos datos del usuario
                _usuario.Nombre = txtNombre.Text.Trim();
                _usuario.Apellidos = txtApellidos.Text.Trim();
                _usuario.Email = txtEmail.Text.Trim();
                _usuario.Telefono = txtTelefono.Text.Trim();

                // Guardamos cambios en base de datos
                if (_usuarioService.Actualizar(_usuario))
                {
                    // Si es veterinario, guardamos datos profesionales
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

        /// <summary>
        /// Guarda o actualiza los datos profesionales del veterinario
        /// </summary>
        /// <param name="idUsuario">ID del usuario asociado</param>
        private void GuardarDatosProfesionales(int idUsuario)
        {
            Veterinario vete = new()
            {
                IdUsuario = idUsuario,
                Especialidad = txtEspecialidad.Text.Trim(),
                NumeroColegiado = txtColegiado.Text.Trim()
            };

            // Intentamos actualizar, si no existe lo insertamos
            if (!_veteService.Actualizar(vete))
                _veteService.Insertar(vete);
        }

        /// <summary>
        /// Comprueba si el usuario tiene rol de veterinario
        /// </summary>
        private bool EsRolVeterinario()
        {
            return !string.IsNullOrEmpty(_usuario.NombreRol) &&
                   _usuario.NombreRol.Trim().Equals("Veterinario", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Valida los campos del formulario antes de guardar
        /// </summary>
        private bool ValidarCampos()
        {
            // Campos obligatorios
            if (string.IsNullOrWhiteSpace(txtNombre.Text)) return MostrarError("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtApellidos.Text)) return MostrarError("Los apellidos son obligatorios.");
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) return MostrarError("El email es obligatorio.");
            if (string.IsNullOrWhiteSpace(txtTelefono.Text)) return MostrarError("El teléfono es obligatorio.");

            // Validación de formato de email
            if (!RegexEmail().IsMatch(txtEmail.Text))
                return MostrarError("El formato del email no es válido.");

            // Validación de teléfono (9 dígitos)
            if (!RegexTelefono().IsMatch(txtTelefono.Text))
                return MostrarError("El teléfono debe tener 9 dígitos numéricos.");

            // Validaciones adicionales para veterinario
            if (EsRolVeterinario())
            {
                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                    return MostrarError("La especialidad es obligatoria.");

                if (!RegexNumColegiado().IsMatch(txtColegiado.Text))
                    return MostrarError("El número de colegiado debe tener 9 dígitos numéricos.");
            }

            return true;
        }

        /// <summary>
        /// Muestra un mensaje de error al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        private static bool MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        /// <summary>
        /// Cierra la ventana sin guardar cambios
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}