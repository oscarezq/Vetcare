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

namespace Vetcare.Presentacion
{
    /// <summary>
    /// Lógica de interacción para SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();

            Loaded += SplashWindow_Loaded;
        }

        private async void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await SimularCarga();

            Login login = new();
            login.Show();

            this.Close();
        }

        private async Task SimularCarga()
        {
            lblEstado.Text = "Conectando al servidor...";
            await Task.Delay(2000);

            lblEstado.Text = "Cargando base de datos...";
            await Task.Delay(3000);

            lblEstado.Text = "Preparando entorno...";
            await Task.Delay(1500);
        }
    }
}
