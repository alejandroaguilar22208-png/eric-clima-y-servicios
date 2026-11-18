using System.Globalization;
using EricClima.Escritorio.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EricClima.Escritorio.Views
{
    public static class PresupuestoPdf
    {
        private const string Empresa = "Eric Clima y Servicios";
        private const string Rubro = "Instalación, reparación y mantenimiento de aires acondicionados";
        private const string Zona = "Bahía Blanca y zona";
        private static readonly CultureInfo Ar = new CultureInfo("es-AR");
        private static readonly string Azul = "#005CAF";

        private static string Pesos(decimal valor) => valor.ToString("C2", Ar);

        public static void Generar(Presupuesto p, string ruta)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(doc => doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(t => t.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(Empresa).FontSize(20).Bold().FontColor(Azul);
                        col.Item().Text(Rubro).FontColor(Colors.Grey.Darken1);
                        col.Item().Text(Zona).FontColor(Colors.Grey.Darken1);
                    });
                    row.ConstantItem(170).AlignRight().Column(col =>
                    {
                        col.Item().AlignRight().Text("PRESUPUESTO").FontSize(16).Bold();
                        col.Item().AlignRight().Text($"N.º {p.Id:D5}");
                        col.Item().AlignRight().Text($"Fecha: {p.Fecha:dd/MM/yyyy}");
                        col.Item().AlignRight().Text($"Válido hasta: {p.Vence:dd/MM/yyyy}").Bold();
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(c =>
                    {
                        c.Item().Text("Cliente").Bold().FontColor(Azul);
                        c.Item().Text(p.ClienteNombre);
                        c.Item().Text($"Dirección: {p.ClienteDireccion}");
                        c.Item().Text($"Teléfono: {p.ClienteTelefono}");
                    });

                    col.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(5);
                            c.RelativeColumn(1.3f);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                        });

                        tabla.Header(h =>
                        {
                            h.Cell().Element(Titulo).Text("Descripción");
                            h.Cell().Element(Titulo).AlignRight().Text("Cant.");
                            h.Cell().Element(Titulo).AlignRight().Text("Precio unit.");
                            h.Cell().Element(Titulo).AlignRight().Text("Subtotal");
                        });

                        foreach (var i in p.Items)
                        {
                            tabla.Cell().Element(Celda).Text(i.Descripcion);
                            tabla.Cell().Element(Celda).AlignRight().Text(i.Cantidad.ToString("0.##", Ar));
                            tabla.Cell().Element(Celda).AlignRight().Text(Pesos(i.PrecioUnitario));
                            tabla.Cell().Element(Celda).AlignRight().Text(Pesos(i.Subtotal()));
                        }
                    });

                    col.Item().AlignRight().Text($"TOTAL: {Pesos(p.Total())}").FontSize(14).Bold().FontColor(Azul);

                    col.Item().PaddingTop(10).Text(t =>
                    {
                        t.Line("Condiciones").Bold();
                        t.Line($"• Este presupuesto es válido hasta el {p.Vence:dd/MM/yyyy}.");
                        t.Line("• Los trabajos realizados tienen 12 meses de garantía.");
                        t.Line("• Los precios pueden variar si cambian las condiciones de la instalación.");
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span($"{Empresa} — Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            })).GeneratePdf(ruta);
        }

        private static IContainer Titulo(IContainer c) =>
            c.Background(Azul).Padding(6).DefaultTextStyle(t => t.FontColor(Colors.White).Bold());

        private static IContainer Celda(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6);
    }
}
