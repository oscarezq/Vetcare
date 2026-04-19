using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

namespace Vetcare.Negocio.Informes
{
    // Clase que representa el documento PDF de una factura
    public class FacturaDocumento : IDocument
    {
        // Instancia de la factura que se va a imprimir
        private readonly Factura _factura;

        // Instancia del cliente asociado a la factura
        private readonly Cliente _cliente;

        // Constructor que recibe los datos necesarios para generar el PDF
        public FacturaDocumento(Factura factura, Cliente cliente)
        {
            _factura = factura;
            _cliente = cliente;
        }

        // Método principal donde se construye el documento PDF
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Márgenes de la página
                page.Margin(40);

                // Color de fondo de la página
                page.PageColor(Colors.White);

                // Estilo de texto por defecto
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI).FontColor(Colors.Black));

                // HEADER (cabecera del documento)
                page.Header().PaddingBottom(30).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        // Parte izquierda del header (logo + nombre empresa)
                        row.RelativeItem().Row(r =>
                        {
                            // Logo de la empresa
                            r.ConstantItem(60).Height(60).Image("Resources/icono.png");

                            // Nombre y descripción de la empresa
                            r.RelativeItem().PaddingLeft(10).Column(c =>
                            {
                                c.Item().Text("VETCARE").FontSize(24).ExtraBold().FontColor(Colors.Orange.Darken4);
                                c.Item().Text("Clínica Veterinaria").FontSize(10).Italic().FontColor(Colors.Grey.Medium);
                            });
                        });

                        // Parte derecha del header (datos de la factura)
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("FACTURA").FontSize(20).Bold(); // Título
                            c.Item().Text($"Nº: {_factura.NumeroFactura}").SemiBold(); // Número de factura
                            c.Item().Text($"Fecha: {_factura.FechaEmision:dd/MM/yyyy}"); // Fecha de emisión
                        });
                    });

                    // Línea separadora inferior
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                // CONTENIDO PRINCIPAL
                page.Content().Column(col =>
                {
                    // Fila con datos del cliente y dirección
                    col.Item().Row(row =>
                    {
                        // Información del cliente
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("CLIENTE").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(2).Text($"{_cliente.Nombre} {_cliente.Apellidos}").FontSize(11).Bold();
                            c.Item().Text($"DNI/NIE: {_cliente.NumDocumento}");
                        });

                        // Dirección de facturación
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("DIRECCIÓN DE FACTURACIÓN").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(2).Text($"{_cliente.CalleDireccion}, {_cliente.NumeroDireccion}");
                            c.Item().Text($"{_cliente.CodigoPostalDireccion} {_cliente.LocalidadDireccion}");
                            c.Item().Text(_cliente.ProvinciaDireccion);
                        });
                    });

                    // TABLA DE DETALLES DE FACTURA
                    col.Item().PaddingTop(30).Table(table =>
                    {
                        // Definición de columnas
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4); // Concepto
                            columns.RelativeColumn(1); // IVA
                            columns.RelativeColumn(2); // Cantidad
                            columns.RelativeColumn(2); // Precio unitario
                            columns.RelativeColumn(2); // Total
                        });

                        // Cabecera de la tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Concepto");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("IVA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Precio U.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Cantidad");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Total");

                            // Estilo de cabecera (línea inferior y negrita)
                            static IContainer HeaderStyle(IContainer container) =>
                                container
                                    .PaddingVertical(10)
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Black)
                                    .DefaultTextStyle(x => x.Bold());
                        });

                        // Filas de datos (cada detalle de la factura)
                        foreach (var item in _factura.Detalles)
                        {
                            table.Cell().Element(CellStyle).Text(item.NombreConcepto);
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.IvaPorcentaje}%");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrecioUnitario:N2}€");
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Cantidad.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalLinea:N2}€").Bold();
                        }

                        // Estilo de las celdas (espaciado y línea inferior)
                        static IContainer CellStyle(IContainer container) =>
                            container
                                .BorderBottom(1)
                                .BorderColor(Colors.Grey.Lighten3)
                                .PaddingTop(5)
                                .PaddingBottom(15)
                                .AlignMiddle();
                    });

                    // SECCIÓN DE TOTALES
                    col.Item().AlignRight().PaddingTop(20).Width(200).Column(c =>
                    {
                        // Base imponible
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Base Imponible:");
                            row.RelativeItem().AlignRight().Text($"{_factura.BaseImponible:N2}€");
                        });

                        // IVA total
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().Text("IVA:");
                            row.RelativeItem().AlignRight().Text($"{_factura.IvaTotal:N2}€");
                        });

                        // Línea separadora
                        c.Item().PaddingVertical(5).LineHorizontal(1);

                        // Total final
                        c.Item().Row(row =>
                        {
                            row.RelativeItem().Text("TOTAL").FontSize(14).Bold();
                            row.RelativeItem().AlignRight().Text($"{_factura.Total:N2}€").FontSize(14).Bold();
                        });
                    });
                });

                // FOOTER (pie de página con numeración)
                page.Footer().PaddingTop(20).AlignCenter().Column(c =>
                {
                    c.Item().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber(); // Número de página actual
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