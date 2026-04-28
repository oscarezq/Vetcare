using System;
using System.Windows;
using System.Windows.Controls;
using Vetcare.Entidades;
using Vetcare.Negocio.Services;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;
using Vetcare.Presentacion.Inicio;
using Vetcare.Presentacion.Servicios;
using Vetcare.Presentacion.Usuarios;

namespace Vetcare.Presentacion
{
    public partial class MainWindow : Window
    {
        private readonly Usuario usuarioActual;

        public MainWindow()
        {
            InitializeComponent();
            UsuarioService usuarioService = new();
            // Simulación de login para desarrollo
            usuarioService.ValidarLogin("admin", "admin", out usuarioActual!);
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
                lblNombreUsuario.Text = Sesion.UsuarioActual?.Username;
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

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(btnInicio);
            FramePrincipal.Content = new PageInicio();
        }

        private void BtnMascotas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageMascotas();
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageClientes();
        }

        private void BtnCitas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageCitas();
        }

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageUsuarios();
        }

        private void BtnServicios_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageServicios();
        }

        private void BtnProductos_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageProductos();
        }

        private void BtnFacturas_Click(object sender, RoutedEventArgs e)
        {
            SeleccionarBoton(sender);
            FramePrincipal.Content = new PageFacturas();
        }

        // --- GESTIÓN DE PERFIL Y SALIDA ---

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Login login = new();
            login.Show();
            this.Close();
        }

        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
        {
            if (btnPerfil.ContextMenu != null)
            {
                btnPerfil.ContextMenu.PlacementTarget = btnPerfil;
                btnPerfil.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btnPerfil.ContextMenu.IsOpen = true;
            }
        }

        private void BtnVerInfo_Click(object sender, RoutedEventArgs e)
        {
            if (usuarioActual != null)
            {
                WindowVerPerfil perfil = new(usuarioActual)
                {
                    Owner = this
                };
                perfil.ShowDialog();
            }
        }

        private void BtnCambiarPass_Click(object sender, RoutedEventArgs e)
        {
            WindowCambiarPassword win = new(usuarioActual, false);
            win.ShowDialog();
        }
    }
}