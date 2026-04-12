using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

public class JustificanteDocumento : IDocument
{
    private readonly Cita _cita;

    public JustificanteDocumento(Cita cita)
    {
        _cita = cita;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Column(col =>
            {
                col.Item().Text("JUSTIFICANTE DE CONSULTA")
                    .FontSize(20)
                    .Bold()
                    .FontColor("#27AE60");

                col.Item().LineHorizontal(1);
            });

            page.Content().Column(col =>
            {
                col.Spacing(12);

                col.Item().Text($"Fecha de atención: {_cita.FechaHora:dd/MM/yyyy HH:mm}");
                col.Item().Text($"Cliente: {_cita.NombreDueno}");
                col.Item().Text($"Mascota: {_cita.NombreMascota}");
                col.Item().Text($"Veterinario: {_cita.NombreVeterinario}");
                col.Item().Text($"Nº Colegiado: {_cita.NumeroColegiado}");

                col.Item().PaddingTop(10).Text("Motivo de la consulta:")
                    .Bold();

                col.Item().Text(_cita.Motivo);

                if (!string.IsNullOrWhiteSpace(_cita.Observaciones))
                {
                    col.Item().PaddingTop(10).Text("Observaciones médicas:")
                        .Bold();

                    col.Item().Text(_cita.Observaciones);
                }

                col.Item().PaddingTop(20).Text("Documento emitido como justificante de asistencia.")
                    .Italic()
                    .FontColor("#7F8C8D");
            });

            page.Footer().AlignRight().Text(x =>
            {
                x.Span("Fecha emisión: ");
                x.Span(DateTime.Now.ToString("dd/MM/yyyy"));
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}