using System;
using QuestPDF.Fluent;              // API fluida para construir documentos PDF
using QuestPDF.Helpers;             // Utilidades como colores, fuentes, etc.
using QuestPDF.Infrastructure;      // Interfaces base como IDocument
using Vetcare.Entidades;            // Acceso a entidades del dominio (Cita)

namespace Vetcare.Negocio.Informes
{

    // Clase que representa el documento PDF de un recordatorio de cita
    public class RecordatorioDocumento : IDocument
    {
        // Datos de la cita que se usarán para generar el recordatorio
        private readonly Cita _cita;

        // Color principal reutilizable para estilos
        private readonly string ColorPrimario = "#2A5C82";

        // Color de fondo gris claro para secciones destacadas
        private readonly string ColorFondoGris = "#F8F9FA";

        // Constructor que recibe la cita
        public RecordatorioDocumento(Cita cita)
        {
            _cita = cita;
        }

        // Método principal donde se construye el documento PDF
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Márgenes de la página
                page.Margin(40);

                // Color de fondo
                page.PageColor(Colors.White);

                // Estilo de texto por defecto
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.SegoeUI));

                // Cabecera
                page.Header().Element(ComposeHeader);

                // Contenido principal
                page.Content().Element(ComposeContent);

                // Pie de página
                page.Footer().Element(ComposeFooter);
            });
        }

        // Método que compone la cabecera del documento
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // Parte izquierda: logo + nombre de la clínica
                row.RelativeItem().Row(r =>
                {
                    // Logo
                    r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                    // Nombre y descripción
                    r.RelativeItem().PaddingLeft(10).Column(c =>
                    {
                        c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                        c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                    });
                });

                // Parte derecha: título y fecha de emisión
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("RECORDATORIO DE CITA").FontSize(14).Bold().FontColor(ColorPrimario);
                    col.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }

        // Método que compone el contenido principal
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(30).Column(col =>
            {
                col.Spacing(20); // Espaciado entre secciones

                // --- SECCIÓN: FECHA Y HORA (destacada) ---
                col.Item().Background(ColorFondoGris).Padding(15).Row(row =>
                {
                    // Columna izquierda: fecha
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("¿Cuándo?").FontSize(10).FontColor(ColorPrimario).Bold();
                        c.Item().Text($"{_cita.FechaHora:dddd, dd 'de' MMMM}").FontSize(16).Medium();
                    });

                    // Columna derecha: hora
                    row.ConstantItem(100).AlignRight().Column(c =>
                    {
                        c.Item().Text("Hora").FontSize(10).FontColor(ColorPrimario).Bold();
                        c.Item().Text($"{_cita.FechaHora:HH:mm}").FontSize(16).Medium();
                    });
                });

                // Duración estimada de la cita
                col.Item().AlignRight().Text(t =>
                {
                    t.Span("Duración estimada: ").FontSize(10).SemiBold();
                    t.Span($"{_cita.DuracionEstimada} min").FontSize(10);
                });

                // --- SECCIÓN: DETALLES ---
                col.Item().Row(row =>
                {
                    // Columna izquierda: información de mascota y propietario
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().BorderBottom(1).BorderColor(ColorPrimario).PaddingBottom(5).Text("INFORMACIÓN").Bold();
                        c.Spacing(5);

                        c.Item().PaddingTop(5).Text(t =>
                        {
                            t.Span("Paciente: ").Bold();
                            t.Span(_cita.NombreMascota);
                        });

                        c.Item().Text(t =>
                        {
                            t.Span("Propietario: ").Bold();
                            t.Span(_cita.NombreDueno);
                        });
                    });

                    // Espaciador entre columnas
                    row.ConstantItem(40);

                    // Columna derecha: información del profesional
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().BorderBottom(1).BorderColor(ColorPrimario).PaddingBottom(5).Text("PROFESIONAL").Bold();
                        c.Spacing(5);
                        c.Item().PaddingTop(5).Text($"Dr. {_cita.NombreVeterinario}");
                    });
                });

                // --- SECCIÓN: MOTIVO DE LA CONSULTA ---
                col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                {
                    c.Item().Text("Motivo de consulta:").FontSize(10).Bold();
                    c.Item().PaddingTop(4).Text(_cita.Motivo).Italic();
                });

                // --- NOTA IMPORTANTE ---
                col.Item().AlignCenter().PaddingTop(20).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Text(t =>
                {
                    t.Span("Nota: ").Bold().FontColor(Colors.Orange.Medium);
                    t.Span("Por favor, llegue 10 minutos antes para el registro. Si no puede asistir, agradecemos nos notifique con 24h de antelación.");
                });
            });
        }

        // Método que compone el pie de página
        private void ComposeFooter(IContainer container)
        {
            container.Column(c =>
            {
                // Línea superior
                c.Item().LineHorizontal(1);

                // Numeración de páginas
                c.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber(); // Página actual
                        x.Span(" de ");
                        x.TotalPages(); // Total de páginas
                    });
                });
            });
        }

        // Metadatos del documento (por defecto)
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}