using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Usuarios;

namespace Vetcare.Presentacion.Veterinarios
{
    /// <summary>
    /// Ventana selector de mascotas para elegir una mascota existente o crear una nueva.
    /// </summary>
    public partial class WindowSelectorVeterinario : Window
    {
        // Lista local de veterinarios filtrados
        private List<Veterinario>? listaVeterinarios;

        // Veterinario seleccionado que se devolverá al cerrar la ventana
        public Veterinario? VeterinarioSeleccionado;

        // Servicios de negocio
        private readonly VeterinarioService veteService = new();
        private readonly UsuarioService usuarioService = new();

        /// <summary>
        /// Constructor principal de la ventana selector
        /// </summary>
        public WindowSelectorVeterinario()
        {
            InitializeComponent();
            CargarLista();
        }

        /// <summary>
        /// Carga la lista de veterinarios activos (usuario activo asociado)
        /// </summary>
        private void CargarLista()
        {
            try
            {
                // Obtener todos los veterinarios desde la BD
                var todosLosVetes = veteService.ObtenerTodos();

                // Filtrar solo veterinarios con usuario activo
                listaVeterinarios = todosLosVetes.Where(v => {
                    var user = usuarioService.ObtenerPorId(v.IdUsuario);
                    return user != null && user.Activo == true;
                }).ToList();

                // Asignar al DataGrid
                dgVeterinarios.ItemsSource = listaVeterinarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar veterinarios: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filtro en tiempo real por texto (nombre, apellidos, especialidad, colegiado)
        /// </summary>
        private void TxtBuscaVeterinario_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listaVeterinarios == null) return;

            var busqueda = txtBuscaVeterinario.Text.ToLower().Trim();

            // Aplicar filtro sobre la lista original
            var filtrados = listaVeterinarios.Where(v =>
                (v.Nombre != null && v.Nombre.ToLower().Contains(busqueda)) ||
                (v.Apellidos != null && v.Apellidos.ToLower().Contains(busqueda)) ||
                (v.Especialidad != null && v.Especialidad.ToLower().Contains(busqueda)) ||
                (v.NumeroColegiado != null && v.NumeroColegiado.ToLower().Contains(busqueda))
            ).ToList();

            dgVeterinarios.ItemsSource = filtrados;
        }

        /// <summary>
        /// Finaliza la selección del veterinario actual
        /// </summary>
        private void FinalizarSeleccion()
        {
            if (dgVeterinarios.SelectedItem is Veterinario vete)
            {
                VeterinarioSeleccionado = vete;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un veterinario de la lista.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Doble click en la tabla
        private void DgVeterinarios_MouseDoubleClick(object sender, MouseButtonEventArgs e) => FinalizarSeleccion();

        // Botón seleccionar
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e) => FinalizarSeleccion();

        // Botón cancelar
        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => this.Close();

        /// <summary>
        /// Abre ventana de creación de usuario/veterinario y recarga lista
        /// </summary>
        private void BtnNuevoVeterinario_Click(object sender, RoutedEventArgs e)
        {
            // Abrir ventana de usuario (posible creación de veterinario)
            WindowUsuario? win = new();

            if (win.ShowDialog() == true)
            {
                // Recargar lista tras crear nuevo veterinario
                CargarLista();
            }
        }
    }
}