using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Informes
{
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

                page.DefaultTextStyle(x => x
                    .FontSize(10)
                    .FontFamily(Fonts.SegoeUI)
                    .FontColor(Colors.Black));

                // HEADER
                page.Header().PaddingBottom(20).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Row(r =>
                        {
                            r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                            r.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().Text("VETCARE")
                                    .FontSize(24)
                                    .ExtraBold()
                                    .FontColor(Colors.Orange.Darken4);

                                c.Item().Text("Clínica Veterinaria")
                                    .FontSize(10)
                                    .Italic()
                                    .FontColor(Colors.Grey.Medium);
                            });
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("INFORME DE CONSULTA")
                                .FontSize(18)
                                .Bold();

                            c.Item().Text($"Fecha: {_cita.FechaHora:dd/MM/yyyy HH:mm}");
                        });
                    });

                    col.Item().PaddingTop(10)
                        .LineHorizontal(1)
                        .LineColor(Colors.Grey.Lighten2);
                });

                // CONTENIDO
                page.Content().Column(col =>
                {
                    // DATOS MASCOTA + PROPIETARIO
                    col.Item().PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("DATOS DE LA MASCOTA")
                                .FontSize(9)
                                .SemiBold()
                                .FontColor(Colors.Orange.Darken4);

                            c.Item().PaddingTop(2).Text($"Nombre: {_mascota.Nombre}")
                                .Bold().FontSize(12);

                            c.Item().Text($"Especie: {_mascota.NombreEspecie}");
                            c.Item().Text($"Raza: {_mascota.NombreRaza}");
                            c.Item().Text($"Sexo: {_mascota.Sexo}");
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("PROPIETARIO")
                                .FontSize(9)
                                .SemiBold()
                                .FontColor(Colors.Grey.Medium);

                            c.Item().PaddingTop(2).Text(_cliente.NombreCompleto).Bold();
                            c.Item().Text($"DNI: {_cliente.NumDocumento}");
                            c.Item().Text($"Tel: {_cliente.Telefono}");
                        });
                    });

                    // DETALLE CONSULTA (bloque tipo tarjeta)
                    col.Item().Text("DETALLE DE LA CONSULTA")
                        .FontSize(14)
                        .Bold();

                    col.Item().PaddingVertical(10)
                        .Background(Colors.Grey.Lighten5)
                        .Padding(10)
                        .Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text($"Veterinario: {_cita.NombreVeterinario}")
                                    .Bold()
                                    .FontColor(Colors.Blue.Darken4);

                                r.RelativeItem().AlignRight().Text($"Nº Colegiado: {_cita.NumeroColegiado}")
                                    .FontSize(9)
                                    .Italic();
                            });

                            if (!string.IsNullOrEmpty(_cita.Motivo))
                            {
                                c.Item().PaddingTop(5).Text(t =>
                                {
                                    t.Span("Motivo: ").Bold();
                                    t.Span(_cita.Motivo);
                                });
                            }
                        });

                    // RESULTADO CLÍNICO
                    col.Item().PaddingTop(15).Text("RESULTADO CLÍNICO")
                        .FontSize(14)
                        .Bold();

                    col.Item().PaddingVertical(10)
                        .Background(Colors.Grey.Lighten5)
                        .Padding(10)
                        .Column(c =>
                        {
                            if (!string.IsNullOrEmpty(_historial.Diagnostico))
                            {
                                c.Item().Row(r =>
                                {
                                    r.ConstantItem(90).Text("Diagnóstico:").Bold();
                                    r.RelativeItem().Text(_historial.Diagnostico);
                                });
                            }

                            if (!string.IsNullOrEmpty(_historial.Tratamiento))
                            {
                                c.Item().PaddingTop(5).Row(r =>
                                {
                                    r.ConstantItem(90).Text("Tratamiento:").Bold();
                                    r.RelativeItem().Text(_historial.Tratamiento);
                                });
                            }

                            if (!string.IsNullOrEmpty(_historial.Observaciones))
                            {
                                c.Item().PaddingTop(5).Text("Observaciones:")
                                    .Bold().FontSize(9);

                                c.Item().Text(_historial.Observaciones)
                                    .FontSize(9)
                                    .FontColor(Colors.Grey.Darken2);
                            }

                            if (_historial.Peso.HasValue)
                            {
                                c.Item().PaddingTop(5)
                                    .AlignRight()
                                    .Text($"Peso: {_historial.Peso} kg")
                                    .FontSize(8)
                                    .Italic();
                            }
                        });
                });

                // FOOTER
                page.Footer().PaddingTop(20).Column(c =>
                {
                    c.Item().LineHorizontal(1);

                    c.Item().PaddingTop(5).Row(row =>
                    {
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
}