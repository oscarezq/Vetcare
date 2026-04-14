using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vetcare.Entidades;

public class FacturaDocumento : IDocument
{
    private readonly Factura _factura;
    private readonly Cliente _cliente;

    public FacturaDocumento(Factura factura, Cliente cliente)
    {
        _factura = factura;
        _cliente = cliente;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI).FontColor(Colors.Black));

            // 🔷 HEADER (Más espacio abajo y sin colores estridentes)
            page.Header().PaddingBottom(30).Column(col =>
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
                        c.Item().Text("FACTURA").FontSize(20).Bold();
                        c.Item().Text($"Nº: {_factura.NumeroFactura}").SemiBold();
                        c.Item().Text($"Fecha: {_factura.FechaEmision:dd/MM/yyyy}");
                    });
                });

                col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            });

            // 👤 SECCIÓN CLIENTE Y DIRECCIÓN (Separados y limpios)
            page.Content().Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("CLIENTE").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text($"{_cliente.Nombre} {_cliente.Apellidos}").FontSize(11).Bold();
                        c.Item().Text($"DNI/NIE: {_cliente.NumDocumento}");
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("DIRECCIÓN DE FACTURACIÓN").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                        c.Item().PaddingTop(2).Text($"{_cliente.CalleDireccion}, {_cliente.NumeroDireccion}");
                        c.Item().Text($"{_cliente.CodigoPostalDireccion} {_cliente.LocalidadDireccion}");
                        c.Item().Text(_cliente.ProvinciaDireccion);
                    });
                });

                // 📊 TABLA (IVA al lado del producto y espaciado corregido)
                col.Item().PaddingTop(30).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4); // Concepto
                        columns.RelativeColumn(1); // IVA
                        columns.RelativeColumn(2); // Cant.
                        columns.RelativeColumn(2); // Precio Un. (Base)
                        columns.RelativeColumn(2); // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Concepto");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("IVA");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Precio U.");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Cantidad");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("Total");

                        static IContainer HeaderStyle(IContainer container) =>
                            container
                                .PaddingVertical(10)
                                .BorderBottom(1)
                                .BorderColor(Colors.Black)
                                .DefaultTextStyle(x => x.Bold());
                    });

                    foreach (var item in _factura.Detalles)
                    {
                        table.Cell().Element(CellStyle).Text(item.NombreConcepto);
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.IvaPorcentaje}%");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.PrecioUnitario:N2}€");
                        table.Cell().Element(CellStyle).AlignRight().Text(item.Cantidad.ToString());
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalLinea:N2}€").Bold();
                    }

                    // Estilo: Menos arriba (5), mucho más abajo (15)
                    static IContainer CellStyle(IContainer container) =>
                        container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten3)
                            .PaddingTop(5)
                            .PaddingBottom(15)
                            .AlignMiddle();
                });

                // 💰 TOTALES
                col.Item().AlignRight().PaddingTop(20).Width(200).Column(c =>
                {
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Base Imponible:");
                        row.RelativeItem().AlignRight().Text($"{_factura.BaseImponible:N2}€");
                    });

                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text("IVA:");
                        row.RelativeItem().AlignRight().Text($"{_factura.IvaTotal:N2}€");
                    });

                    c.Item().PaddingVertical(5).LineHorizontal(1);

                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL").FontSize(14).Bold();
                        row.RelativeItem().AlignRight().Text($"{_factura.Total:N2}€").FontSize(14).Bold();
                    });
                });
            });

            // 🔻 FOOTER
            page.Footer().PaddingTop(20).AlignCenter().Column(c => {
                c.Item().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}