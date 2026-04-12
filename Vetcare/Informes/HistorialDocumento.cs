using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

public class HistorialDocumento : IDocument
{
    private readonly Mascota _mascota;
    private readonly List<HistorialClinico> _historiales;

    public HistorialDocumento(Mascota mascota, List<HistorialClinico> historiales)
    {
        _mascota = mascota;
        _historiales = historiales;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI).FontColor(Colors.Black));

            // 🔷 HEADER: Información de la Clínica
            page.Header().PaddingBottom(20).Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Row(r =>
                    {
                        r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                        r.RelativeItem().PaddingLeft(10).Column(c =>
                        {
                            c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                            c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                        });
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("HISTORIAL CLÍNICO").FontSize(18).Bold();
                        c.Item().Text($"Fecha de Reporte: {DateTime.Now:dd/MM/yyyy}");
                    });
                });
                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });

            page.Content().Column(col =>
            {
                // 🐾 SECCIÓN 1: DATOS DE LA MASCOTA Y DUEÑO
                col.Item().PaddingBottom(20).Row(row =>
                {
                    // Datos Mascota
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("DATOS DE LA MASCOTA").FontSize(9).SemiBold().FontColor(Colors.Orange.Darken4);
                        c.Item().PaddingTop(2).Text($"Nombre: {_mascota.Nombre}").Bold().FontSize(12);
                        c.Item().Text($"Especie: {_mascota.NombreEspecie}");
                        c.Item().Text($"Raza: {_mascota.NombreRaza}");
                        c.Item().Text($"Sexo: {_mascota.Sexo}");
                    });

                    // Datos Dueño
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("PROPIETARIO").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text($"{_mascota.NombreDueno} {_mascota.ApellidosDueno}").Bold();
                        c.Item().Text($"DNI: {_mascota.NumeroIdentificacionDueno}");
                    });
                });

                // 📋 SECCIÓN 2: ENTRADAS DEL HISTORIAL
                col.Item().Text("DETALLES CLÍNICOS").FontSize(14).Bold().FontColor(Colors.Black);
                col.Item().PaddingTop(5).PaddingBottom(10).LineHorizontal(0.5f).LineColor(Colors.Black);

                if (_historiales == null || _historiales.Count == 0)
                {
                    col.Item().PaddingTop(20).AlignCenter().Text("No existen registros médicos previos para esta mascota.").Italic();
                }
                else
                {
                    foreach (var h in _historiales)
                    {
                        col.Item().PaddingVertical(10).Background(Colors.Grey.Lighten5).Padding(10).Column(c =>
                        {
                            // Encabezado de la consulta
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text($"Fecha: {h.FechaHora:dd/MM/yyyy HH:mm}").Bold().FontColor(Colors.Orange.Darken4);
                                r.RelativeItem().AlignRight().Text($"Veterinario: {h.NombreVeterinario}").Italic().FontSize(9);
                            });

                            if (!string.IsNullOrEmpty(h.Motivo))
                                c.Item().PaddingTop(5).Text(t => { t.Span("Motivo: ").Bold(); t.Span(h.Motivo); });

                            c.Item().PaddingTop(5).Row(r => {
                                r.ConstantItem(80).Text("Diagnóstico:").Bold();
                                r.RelativeItem().Text(h.Diagnostico);
                            });

                            c.Item().PaddingTop(2).Row(r => {
                                r.ConstantItem(80).Text("Tratamiento:").Bold();
                                r.RelativeItem().Text(h.Tratamiento);
                            });

                            if (!string.IsNullOrEmpty(h.Observaciones))
                            {
                                c.Item().PaddingTop(5).Text("Observaciones adicionales:").Bold().FontSize(9);
                                c.Item().Text(h.Observaciones).FontSize(9).FontColor(Colors.Grey.Darken2);
                            }

                            c.Item().PaddingTop(5).AlignRight().Text($"Peso registrado: {h.Peso} kg").FontSize(8).Italic();
                        });

                        // Separador entre consultas
                        col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                    }
                }
            });

            // 🔻 FOOTER
            page.Footer().PaddingTop(20).Column(c =>
            {
                c.Item().LineHorizontal(1);
                c.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("Documento oficial de VetCare - ").FontSize(9);
                        x.Span(DateTime.Now.ToString("yyyy")).FontSize(9);
                    });
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