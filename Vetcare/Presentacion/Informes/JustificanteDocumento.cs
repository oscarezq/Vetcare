using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

public class JustificanteDocumento : IDocument
{
    private readonly Cita _cita;
    private readonly string ColorPrimario = "#27AE60";
    private readonly string ColorFondoGris = "#F8F9FA";

    public JustificanteDocumento(Cita cita)
    {
        _cita = cita;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.SegoeUI).LineHeight(1.5f));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(60).Height(60).Image("Resources/icono.png");

            row.RelativeItem().PaddingLeft(10).Column(c =>
            {
                c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                col.Item().Text("CERTIFICADO DE ASISTENCIA").FontSize(14).Bold().FontColor(Colors.Blue.Darken4);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(col =>
        {
            col.Spacing(20);

            // --- CUERPO DEL TEXTO JUSTIFICATIVO ---
            col.Item().Text(t =>
            {
                t.Span("D./Dña. ");
                t.Span($"{_cita.NombreVeterinario}").Bold();
                t.Span(", facultativo colegiado número ");
                t.Span($"{_cita.NumeroColegiado}").Bold();
                t.Span(", en representación de la clínica VETCARE,");
            });

            col.Item().Text("CERTIFICA:").Bold().FontSize(12);

            col.Item().Background(ColorFondoGris).Padding(20).Text(t =>
            {
                t.Span("Que el cliente ");
                t.Span($"{_cita.NombreDueno}").Bold();
                t.Span(", responsable del paciente (mascota) ");
                t.Span($"{_cita.NombreMascota}").Bold();
                t.Span(", ha permanecido en nuestras instalaciones para consulta veterinaria el día ");
                t.Span($"{_cita.FechaHora:dd/MM/yyyy}").Bold();
                t.Span(", desde las ");
                t.Span($"{_cita.FechaHora:HH:mm}").Bold();
                t.Span(" horas hasta las ");
                t.Span($"{DateTime.Now:HH:mm}").Bold();
                t.Span(" horas (hora de emisión del presente documento).");
            });

            // --- SECCIÓN DE FIRMA ---
            col.Item().PaddingTop(50).Row(row =>
            {
                row.RelativeItem(); // Espacio a la izquierda

                row.ConstantItem(200).Column(c =>
                {
                    c.Item().AlignCenter().Text("Firma y sello del centro:").FontSize(10);
                    c.Item().PaddingVertical(10).Height(60).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1);
                    c.Item().AlignCenter().Text($"Dr. {_cita.NombreVeterinario}").FontSize(9);
                });
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}