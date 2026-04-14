using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using Vetcare.Entidades;

public class InformeConsultaDocumento : IDocument
{
    private readonly Mascota _mascota;
    private readonly Cliente _cliente;
    private readonly Cita _cita;
    private readonly HistorialClinico _historial;

    public InformeConsultaDocumento(
        Mascota mascota,
        Cliente cliente,
        Cita cita,
        HistorialClinico historial)
    {
        _mascota = mascota;
        _cliente = cliente;
        _cita = cita;
        _historial = historial;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI));

            // 🔷 HEADER
            page.Header().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Row(r =>
                    {
                        r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                        r.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text("VETCARE").FontSize(22).Bold().FontColor(Colors.Blue.Darken3);
                            c.Item().Text("Clínica Veterinaria").FontSize(10).Italic();
                        });
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("INFORME DE CONSULTA").FontSize(16).Bold();
                        c.Item().Text($"Fecha: {_cita.FechaHora:dd/MM/yyyy HH:mm}");
                    });
                });

                col.Item().PaddingTop(10).LineHorizontal(1);
            });

            // 📄 CONTENIDO
            page.Content().PaddingVertical(15).Column(col =>
            {
                // 🐾 DATOS MASCOTA
                col.Item().Text("DATOS DE LA MASCOTA").Bold().FontColor(Colors.Blue.Darken3);

                col.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Nombre: {_mascota.Nombre}").Bold();
                        c.Item().Text($"Especie: {_mascota.NombreEspecie}");
                        c.Item().Text($"Raza: {_mascota.NombreRaza}");
                        c.Item().Text($"Sexo: {_mascota.Sexo}");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text($"Propietario: {_cliente.NombreCompleto}").Bold();
                        c.Item().Text($"Documento: {_cliente.NumDocumento}");
                        c.Item().Text($"Teléfono: {_cliente.Telefono}");
                    });
                });

                col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                // 🩺 DATOS CONSULTA
                col.Item().Text("DETALLE DE LA CONSULTA").Bold().FontColor(Colors.Blue.Darken3);

                col.Item().PaddingTop(5).Text(t =>
                {
                    t.Span("Veterinario: ").Bold();
                    t.Span(_cita.NombreVeterinario);
                });

                col.Item().Text(t =>
                {
                    t.Span("Nº Colegiado: ").Bold();
                    t.Span(_cita.NumeroColegiado);
                });

                if (!string.IsNullOrEmpty(_cita.Motivo))
                {
                    col.Item().PaddingTop(5).Text(t =>
                    {
                        t.Span("Motivo de la consulta: ").Bold();
                        t.Span(_cita.Motivo);
                    });
                }

                col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                // 📋 RESULTADO CLÍNICO
                col.Item().Text("RESULTADO CLÍNICO").Bold().FontColor(Colors.Blue.Darken3);

                if (!string.IsNullOrEmpty(_historial.Diagnostico))
                {
                    col.Item().PaddingTop(5).Text("Diagnóstico:").Bold();
                    col.Item().Text(_historial.Diagnostico);
                }

                if (!string.IsNullOrEmpty(_historial.Tratamiento))
                {
                    col.Item().PaddingTop(5).Text("Tratamiento:").Bold();
                    col.Item().Text(_historial.Tratamiento);
                }

                if (!string.IsNullOrEmpty(_historial.Observaciones))
                {
                    col.Item().PaddingTop(5).Text("Observaciones:").Bold();
                    col.Item().Text(_historial.Observaciones).FontColor(Colors.Grey.Darken2);
                }

                if (_historial.Peso.HasValue)
                {
                    col.Item().PaddingTop(5).Text($"Peso registrado: {_historial.Peso} kg").Italic();
                }
            });

            // 🔻 FOOTER
            page.Footer().Column(col =>
            {
                col.Item().LineHorizontal(1);

                col.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("Gracias por confiar en nuestra clínica veterinaria.");
                    row.RelativeItem().AlignRight().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}