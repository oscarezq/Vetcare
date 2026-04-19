using System;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Informes
{

    // Clase que representa el documento PDF de un justificante de asistencia
    public class JustificanteDocumento : IDocument
    {
        // Datos de la cita que se utilizarán para generar el justificante
        private readonly Cita _cita;

        // Constructor que recibe la cita
        public JustificanteDocumento(Cita cita)
        {
            _cita = cita;
        }

        // Método principal donde se construye el documento PDF
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Márgenes de la página
                page.Margin(50);

                // Color de fondo
                page.PageColor(Colors.White);

                // Estilo de texto por defecto
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.SegoeUI).LineHeight(1.5f));

                // Cabecera del documento
                page.Header().Element(ComposeHeader);

                // Contenido principal del documento
                page.Content().Element(ComposeContent);
            });
        }

        // Método que compone la cabecera
        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                // Logo de la clínica
                row.ConstantItem(60).Height(60).Image("Resources/icono.png");

                // Nombre de la clínica
                row.RelativeItem().PaddingLeft(10).Column(c =>
                {
                    c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                    c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                });

                // Título del documento (alineado a la derecha)
                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text("CERTIFICADO DE ASISTENCIA").FontSize(14).Bold().FontColor(Colors.Blue.Darken4);
                });
            });
        }

        // Método que compone el contenido principal
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(col =>
            {
                col.Spacing(20); // Espaciado entre elementos

                // --- CUERPO DEL TEXTO JUSTIFICATIVO ---
                col.Item().Text(t =>
                {
                    t.Span("D./Dña. ");
                    t.Span($"{_cita.NombreVeterinario}").Bold(); // Nombre del veterinario
                    t.Span(", facultativo colegiado número ");
                    t.Span($"{_cita.NumeroColegiado}").Bold(); // Número de colegiado
                    t.Span(", en representación de la clínica VETCARE,");
                });

                // Texto "CERTIFICA"
                col.Item().Text("CERTIFICA:").Bold().FontSize(12);

                // Bloque principal del certificado con fondo gris
                col.Item().Background("#F8F9FA").Padding(20).Text(t =>
                {
                    t.Span("Que el cliente ");
                    t.Span($"{_cita.NombreDueno}").Bold(); // Nombre del cliente
                    t.Span(", responsable del paciente (mascota) ");
                    t.Span($"{_cita.NombreMascota}").Bold(); // Nombre de la mascota
                    t.Span(", ha permanecido en nuestras instalaciones para consulta veterinaria el día ");
                    t.Span($"{_cita.FechaHora:dd/MM/yyyy}").Bold(); // Fecha de la consulta
                    t.Span(", desde las ");
                    t.Span($"{_cita.FechaHora:HH:mm}").Bold(); // Hora de inicio
                    t.Span(" horas hasta las ");
                    t.Span($"{DateTime.Now:HH:mm}").Bold(); // Hora de emisión del documento
                    t.Span(" horas (hora de emisión del presente documento).");
                });

                // --- SECCIÓN DE FIRMA ---
                col.Item().PaddingTop(50).Row(row =>
                {
                    row.RelativeItem(); // Espacio vacío a la izquierda

                    // Bloque de firma
                    row.ConstantItem(200).Column(c =>
                    {
                        c.Item().AlignCenter().Text("Firma y sello del centro:").FontSize(10);

                        // Línea para firma
                        c.Item().PaddingVertical(10).Height(60).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1);

                        // Nombre del veterinario
                        c.Item().AlignCenter().Text($"Dr. {_cita.NombreVeterinario}").FontSize(9);
                    });
                });
            });
        }

        // Metadatos del documento (por defecto)
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    }
}