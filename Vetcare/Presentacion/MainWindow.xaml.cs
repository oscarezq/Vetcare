using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;
using Vetcare.Presentacion.Inicio;
using Vetcare.Presentacion.Mascotas;
using Vetcare.Presentacion.Servicios;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Presentacion.Veterinarios;

namespace Vetcare.Presentacion
{
    public partial class MainWindow : Window
    {
        private Usuario usuarioActual;

        public MainWindow()
        {
            InitializeComponent();
            UsuarioService usuarioService = new UsuarioService();
            // Simulación de login para desarrollo
            usuarioService.ValidarLogin("admin", "admin", out usuarioActual);
            Sesion.UsuarioActual = usuarioActual;

            FramePrincipal.Content = new PageInicio();
            CargarDatosUsuario();
        }

        public MainWindow(Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;

            FramePrincipal.Content = new PageInicio();
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            if (usuarioActual != null)
            {
                lblNombreUsuario.Text = Sesion.UsuarioActual.Username;
            }
        }

        /// <summary>
        /// Método para gestionar el estado visual de los botones del menú lateral
        /// </summary>
        private void SeleccionarBoton(object sender)
        {
            // Recorremos todos los elementos del StackPanel del menú
            foreach (var child in pnlMenu.Children)
            {
                if (child is Button btn)
                {
                    // Quitamos la marca de "Selected" a todos
                    btn.Tag = null;
                }
            }

            // Aplicamos la marca de "Selected" solo al botón que se pulsó
            if (sender is Button botonPulsado)
            {
                botonPulsado.Tag = "Selected";
            }
        }

        // --- EVENTOS DE NAVEGACIÓN ---

        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(btnInicio);
            FramePrincipal.Content = new PageInicio();
        }

        private void btnMascotas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageMascotas();
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageClientes();
        }

        private void btnCitas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageCitas();
        }

        private void btnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageUsuarios();
        }

        private void btnVeterinarios_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageVeterinarios();
        }

        private void btnServicios_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageServicios();
        }

        private void btnProductos_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageProductos();
        }

        private void btnFacturas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageFacturas();
        }

        // --- GESTIÓN DE PERFIL Y SALIDA ---

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void btnPerfil_Click(object sender, RoutedEventArgs e)
        {
            if (btnPerfil.ContextMenu != null)
            {
                btnPerfil.ContextMenu.PlacementTarget = btnPerfil;
                btnPerfil.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btnPerfil.ContextMenu.IsOpen = true;
            }
        }

        private void btnVerInfo_Click(object sender, RoutedEventArgs e)
        {
            if (usuarioActual != null)
            {
                WindowVerPerfil perfil = new WindowVerPerfil(usuarioActual);
                perfil.Owner = this;
                perfil.ShowDialog();
            }
        }

        private void btnCambiarPass_Click(object sender, RoutedEventArgs e)
        {
            WindowCambiarPassword win = new WindowCambiarPassword(usuarioActual, false);
            win.ShowDialog();
        }
    }
}