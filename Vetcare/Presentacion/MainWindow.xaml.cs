using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Citas;
using Vetcare.Presentacion.Clientes;
using Vetcare.Presentacion.Facturas;
using Vetcare.Presentacion.Mascotas;
using Vetcare.Presentacion.Servicios;
using Vetcare.Presentacion.Usuarios;
using Vetcare.Presentacion.Veterinarios;

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Usuario usuarioActual;

        public MainWindow()
        {
            InitializeComponent();
            UsuarioService usuarioService = new UsuarioService();
            var user = usuarioService.ValidarLogin("admin", "admin", out usuarioActual);

            CargarDatosUsuario();
        }

        public MainWindow(Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;

            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            if (usuarioActual != null)
            {
                // Mostramos Nombre y Apellidos. Si prefieres el username, usa usuarioActual.Username
                lblNombreUsuario.Text = $"{usuarioActual.Username}";
            }
        }

        // --- EVENTOS DE NAVEGACIÓN ---
        private void btnInicio_Click(object sender, RoutedEventArgs e)
        {
            //FramePrincipal.Content = new PageInicio();
        }

        private void btnMascotas_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageMascotas();
        }

        private void btnClientes_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageClientes();
        }

        private void btnCitas_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageCitas();
        }

        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            //FramePrincipal.Content = new PageHistorialClinico();
        }

        private void btnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageUsuarios();
        }

        private void btnVeterinarios_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageVeterinarios();
        }

        private void btnServicios_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageServicios();
        }

        private void btnProductos_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageProductos();
        }

        private void btnFacturas_Click(object sender, RoutedEventArgs e)
        {
            FramePrincipal.Content = new PageFacturas();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        // Abre el menú al hacer clic izquierdo en el nombre de usuario
        private void btnPerfil_Click(object sender, RoutedEventArgs e)
        {
            if (btnPerfil.ContextMenu != null)
            {
                // Posicionamos el menú justo debajo del botón
                btnPerfil.ContextMenu.PlacementTarget = btnPerfil;
                btnPerfil.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                btnPerfil.ContextMenu.IsOpen = true;
            }
        }

        // Acción: Ver Información
        private void btnVerInfo_Click(object sender, RoutedEventArgs e)
        {
            if (usuarioActual != null)
            {
                WindowVerPerfil perfil = new WindowVerPerfil(usuarioActual);
                perfil.Owner = Window.GetWindow(this);
                perfil.ShowDialog();
            }
        }

        // Acción: Cambiar Contraseña
        private void btnCambiarPass_Click(object sender, RoutedEventArgs e)
        {
            WindowCambiarPassword win = new WindowCambiarPassword(usuarioActual, false);
            win.ShowDialog();
        }
    }
}