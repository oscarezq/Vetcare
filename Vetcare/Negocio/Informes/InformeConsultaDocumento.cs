using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Informes
{

    // Clase que representa el documento PDF de un informe de consulta veterinaria
    public class InformeConsultaDocumento : IDocument
    {
        // Datos de la mascota
        private readonly Mascota _mascota;

        // Datos del cliente (propietario)
        private readonly Cliente _cliente;

        // Datos de la cita
        private readonly Cita _cita;

        // Datos clínicos (historial generado tras la consulta)
        private readonly HistorialClinico _historial;

        // Constructor que recibe toda la información necesaria para generar el informe
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
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI));

                // HEADER (cabecera del documento)
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        // Parte izquierda: logo + nombre clínica
                        row.RelativeItem().Row(r =>
                        {
                            // Logo de la clínica
                            r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                            // Nombre y subtítulo
                            r.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().Text("VETCARE").FontSize(22).Bold().FontColor(Colors.Blue.Darken3);
                                c.Item().Text("Clínica Veterinaria").FontSize(10).Italic();
                            });
                        });

                        // Parte derecha: título del informe y fecha de la cita
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("INFORME DE CONSULTA").FontSize(16).Bold();
                            c.Item().Text($"Fecha: {_cita.FechaHora:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // Línea separadora inferior
                    col.Item().PaddingTop(10).LineHorizontal(1);
                });

                // CONTENIDO PRINCIPAL
                page.Content().PaddingVertical(15).Column(col =>
                {
                    // DATOS DE LA MASCOTA
                    col.Item().Text("DATOS DE LA MASCOTA").Bold().FontColor(Colors.Blue.Darken3);

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        // Información de la mascota
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Nombre: {_mascota.Nombre}").Bold();
                            c.Item().Text($"Especie: {_mascota.NombreEspecie}");
                            c.Item().Text($"Raza: {_mascota.NombreRaza}");
                            c.Item().Text($"Sexo: {_mascota.Sexo}");
                        });

                        // Información del propietario
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Propietario: {_cliente.NombreCompleto}").Bold();
                            c.Item().Text($"Documento: {_cliente.NumDocumento}");
                            c.Item().Text($"Teléfono: {_cliente.Telefono}");
                        });
                    });

                    // Línea separadora
                    col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                    // DATOS DE LA CONSULTA
                    col.Item().Text("DETALLE DE LA CONSULTA").Bold().FontColor(Colors.Blue.Darken3);

                    // Veterinario que atiende la consulta
                    col.Item().PaddingTop(5).Text(t =>
                    {
                        t.Span("Veterinario: ").Bold();
                        t.Span(_cita.NombreVeterinario);
                    });

                    // Número de colegiado del veterinario
                    col.Item().Text(t =>
                    {
                        t.Span("Nº Colegiado: ").Bold();
                        t.Span(_cita.NumeroColegiado);
                    });

                    // Motivo de la consulta (si existe)
                    if (!string.IsNullOrEmpty(_cita.Motivo))
                    {
                        col.Item().PaddingTop(5).Text(t =>
                        {
                            t.Span("Motivo de la consulta: ").Bold();
                            t.Span(_cita.Motivo);
                        });
                    }

                    // Línea separadora
                    col.Item().PaddingVertical(10).LineHorizontal(0.5f);

                    // RESULTADO CLÍNICO
                    col.Item().Text("RESULTADO CLÍNICO").Bold().FontColor(Colors.Blue.Darken3);

                    // Diagnóstico (si existe)
                    if (!string.IsNullOrEmpty(_historial.Diagnostico))
                    {
                        col.Item().PaddingTop(5).Text("Diagnóstico:").Bold();
                        col.Item().Text(_historial.Diagnostico);
                    }

                    // Tratamiento (si existe)
                    if (!string.IsNullOrEmpty(_historial.Tratamiento))
                    {
                        col.Item().PaddingTop(5).Text("Tratamiento:").Bold();
                        col.Item().Text(_historial.Tratamiento);
                    }

                    // Observaciones adicionales (si existen)
                    if (!string.IsNullOrEmpty(_historial.Observaciones))
                    {
                        col.Item().PaddingTop(5).Text("Observaciones:").Bold();
                        col.Item().Text(_historial.Observaciones).FontColor(Colors.Grey.Darken2);
                    }

                    // Peso registrado (si existe valor)
                    if (_historial.Peso.HasValue)
                    {
                        col.Item().PaddingTop(5).Text($"Peso registrado: {_historial.Peso} kg").Italic();
                    }
                });

                // FOOTER (pie de página)
                page.Footer().Column(col =>
                {
                    // Línea superior
                    col.Item().LineHorizontal(1);

                    col.Item().PaddingTop(5).Row(row =>
                    {
                        // Mensaje de agradecimiento
                        row.RelativeItem().Text("Gracias por confiar en nuestra clínica veterinaria.");

                        // Numeración de páginas
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