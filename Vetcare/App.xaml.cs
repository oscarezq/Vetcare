using System.Windows;
using QuestPDF.Infrastructure;

namespace Vetcare
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 1. Configurar la licencia de QuestPDF antes de que se abra cualquier ventana
            QuestPDF.Settings.License = LicenseType.Community;

            // 2. Ejecutar el arranque normal de la aplicación
            base.OnStartup(e);
        }
    }
}