using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Informes
{
    // Clase que representa el documento PDF del historial clínico de una mascota
    public class HistorialDocumento : IDocument
    {
        // Mascota a la que pertenece el historial
        private readonly Mascota _mascota;

        // Lista de registros clínicos de la mascota
        private readonly List<HistorialClinico> _historiales;

        // Constructor que recibe los datos necesarios para generar el documento
        public HistorialDocumento(Mascota mascota, List<HistorialClinico> historiales)
        {
            _mascota = mascota;
            _historiales = historiales;
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
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI).FontColor(Colors.Black));

                // HEADER: Información de la clínica
                page.Header().PaddingBottom(20).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        // Parte izquierda: logo + nombre clínica
                        row.RelativeItem().Row(r =>
                        {
                            // Logo
                            r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                            // Nombre y subtítulo
                            r.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                                c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                            });
                        });

                        // Parte derecha: título del documento y fecha
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("HISTORIAL CLÍNICO").FontSize(18).Bold();
                            c.Item().Text($"Fecha de Reporte: {DateTime.Now:dd/MM/yyyy}");
                        });
                    });

                    // Línea separadora inferior
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                // CONTENIDO PRINCIPAL
                page.Content().Column(col =>
                {
                    // SECCIÓN 1: DATOS DE LA MASCOTA Y PROPIETARIO
                    col.Item().PaddingBottom(20).Row(row =>
                    {
                        // Datos de la mascota
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("DATOS DE LA MASCOTA").FontSize(9).SemiBold().FontColor(Colors.Orange.Darken4);
                            c.Item().PaddingTop(2).Text($"Nombre: {_mascota.Nombre}").Bold().FontSize(12);
                            c.Item().Text($"Especie: {_mascota.NombreEspecie}");
                            c.Item().Text($"Raza: {_mascota.NombreRaza}");
                            c.Item().Text($"Sexo: {_mascota.Sexo}");
                        });

                        // Datos del propietario
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("PROPIETARIO").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(2).Text($"{_mascota.NombreDueno} {_mascota.ApellidosDueno}").Bold();
                            c.Item().Text($"DNI: {_mascota.NumeroIdentificacionDueno}");
                        });
                    });

                    // SECCIÓN 2: DETALLES DEL HISTORIAL
                    col.Item().Text("DETALLES CLÍNICOS").FontSize(14).Bold().FontColor(Colors.Black);

                    // Si no hay registros, mostrar mensaje
                    if (_historiales == null || _historiales.Count == 0)
                    {
                        col.Item().PaddingTop(20).AlignCenter().Text("No existen registros médicos previos para esta mascota.").Italic();
                    }
                    else
                    {
                        // Recorrer cada entrada del historial clínico
                        foreach (var h in _historiales)
                        {
                            // Contenedor de cada consulta
                            col.Item().PaddingVertical(10).Background(Colors.Grey.Lighten5).Padding(10).Column(c =>
                            {
                                // Encabezado con fecha y veterinario
                                c.Item().Row(r =>
                                {
                                    r.RelativeItem().Text($"Fecha: {h.FechaHora:dd/MM/yyyy HH:mm}").Bold().FontColor(Colors.Blue.Darken4);
                                    r.RelativeItem().AlignRight().Text($"Veterinario: {h.NombreVeterinario}").Italic().FontSize(9);
                                });

                                // Motivo de la consulta (si existe)
                                if (!string.IsNullOrEmpty(h.Motivo))
                                    c.Item().PaddingTop(5).Text(t => { t.Span("Motivo de la cita: ").Bold(); t.Span(h.Motivo); });

                                // Diagnóstico
                                c.Item().PaddingTop(5).Row(r =>
                                {
                                    r.ConstantItem(80).Text("Diagnóstico:").Bold();
                                    r.RelativeItem().Text(h.Diagnostico);
                                });

                                // Tratamiento
                                c.Item().PaddingTop(2).Row(r =>
                                {
                                    r.ConstantItem(80).Text("Tratamiento:").Bold();
                                    r.RelativeItem().Text(h.Tratamiento);
                                });

                                // Observaciones adicionales (si existen)
                                if (!string.IsNullOrEmpty(h.Observaciones))
                                {
                                    c.Item().PaddingTop(5).Text("Observaciones adicionales:").Bold().FontSize(9);
                                    c.Item().Text(h.Observaciones).FontSize(9).FontColor(Colors.Grey.Darken2);
                                }

                                // Peso registrado
                                c.Item().PaddingTop(5).AlignRight().Text($"Peso registrado: {h.Peso} kg").FontSize(8).Italic();
                            });

                            // Línea separadora entre consultas
                            col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        }
                    }
                });

                // FOOTER (pie de página)
                page.Footer().PaddingTop(20).Column(c =>
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
            });
        }

        // Metadatos del documento (por defecto)
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}