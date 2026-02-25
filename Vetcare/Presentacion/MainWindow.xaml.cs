using System.Windows;
using Vetcare.Entidades;
using Vetcare.Negocio;
using Vetcare.Presentacion.Clientes;

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
        }

        public MainWindow(Usuario user)
        {
            InitializeComponent();
            usuarioActual = user;
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
            //FramePrincipal.Content = new PageCitas();
        }

        private void btnHistorial_Click(object sender, RoutedEventArgs e)
        {
            //FramePrincipal.Content = new PageHistorial(usuarioActual);
        }

        private void btnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            //FramePrincipal.Content = new PageUsuarios();
        }

        private void btnVeterinarios_Click(object sender, RoutedEventArgs e)
        {
            //FramePrincipal.Content = new PageVeterinarios();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}