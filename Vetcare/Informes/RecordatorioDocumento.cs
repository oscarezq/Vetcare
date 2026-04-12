using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

public class RecordatorioDocumento : IDocument
{
    private readonly Cita _cita;
    // Definimos colores para reutilizar
    private readonly string ColorPrimario = "#2A5C82";
    private readonly string ColorFondoGris = "#F8F9FA";

    public RecordatorioDocumento(Cita cita)
    {
        _cita = cita;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.SegoeUI));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("VETCARE").FontSize(28).ExtraBold().FontColor(ColorPrimario);
                col.Item().Text("Cuidado profesional para tu mascota").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text("RECORDATORIO DE CITA").FontSize(14).SemiBold().FontColor(ColorPrimario);
                col.Item().Text($"Fecha de emisión: {DateTime.Now:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(30).Column(col =>
        {
            col.Spacing(20);

            // --- SECCIÓN: FECHA Y HORA (Destacado) ---
            col.Item().Background(ColorFondoGris).Padding(15).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("¿Cuándo?").FontSize(10).FontColor(ColorPrimario).Bold();
                    c.Item().Text($"{_cita.FechaHora:dddd, dd 'de' MMMM}").FontSize(16).Medium();
                });

                row.ConstantItem(100).AlignRight().Column(c =>
                {
                    c.Item().Text("Hora").FontSize(10).FontColor(ColorPrimario).Bold();
                    c.Item().Text($"{_cita.FechaHora:HH:mm}").FontSize(16).Medium();
                });
            });

            // --- SECCIÓN: DETALLES ---
            col.Item().Row(row =>
            {
                // Columna Mascota/Dueño
                row.RelativeItem().Column(c =>
                {
                    c.Item().BorderBottom(1).BorderColor(ColorPrimario).PaddingBottom(5).Text("INFORMACIÓN").Bold();
                    c.Spacing(5);
                    c.Item().PaddingTop(5).Text(t => {
                        t.Span("Paciente: ").Bold();
                        t.Span(_cita.NombreMascota);
                    });
                    c.Item().Text(t => {
                        t.Span("Propietario: ").Bold();
                        t.Span(_cita.NombreDueno);
                    });
                });

                row.ConstantItem(40); // Espaciador

                // Columna Profesional
                row.RelativeItem().Column(c =>
                {
                    c.Item().BorderBottom(1).BorderColor(ColorPrimario).PaddingBottom(5).Text("PROFESIONAL").Bold();
                    c.Spacing(5);
                    c.Item().PaddingTop(5).Text($"Dr. {_cita.NombreVeterinario}");
                    c.Item().Text($"Duración: {_cita.DuracionEstimada} min").FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            // --- SECCIÓN: MOTIVO ---
            col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
            {
                c.Item().Text("Motivo de consulta:").FontSize(10).Bold();
                c.Item().PaddingTop(4).Text(_cita.Motivo).Italic();
            });

            // --- NOTA DE AVISO ---
            col.Item().AlignCenter().PaddingTop(20).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Text(t =>
            {
                t.Span("Nota: ").Bold().FontColor(Colors.Orange.Medium);
                t.Span("Por favor, llegue 10 minutos antes para el registro. Si no puede asistir, agradecemos nos notifique con 24h de antelación.");
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(c =>
        {
            c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            c.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text("Calle Veterinaria 123, Ciudad").FontSize(9);
                row.RelativeItem().AlignCenter().Text("Tlf: +34 900 000 000").FontSize(9);
                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Página ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9);
                });
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}