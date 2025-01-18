using DescuentoDamasco.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SAPbouiCOM;
using System.Data.Common;
using static DescuentoDamasco.Models.Comprobante.DespachoModel;

namespace DescuentoDamasco.Controllers
{
    public class GuiaDespacho : IDocument
    {
        public static Image DamascoLogo { get; } = Image.FromFile("Images/logodamascorojo.png");
        public FletyCliente _fletyCliente { get; set; }

        public GuiaDespacho(FletyCliente fletyCliente)
        {


            _fletyCliente = fletyCliente;
        }


        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);



            });
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void ComposeHeader(IContainer container)
        {
            container.Column(col => {

                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(col => {
                        col.Item().Text($"Factura #{_fletyCliente.NumFactura}")
                     .FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);

                        col.Item().Text(text => {

                            text.Span("Fecha:").SemiBold();
                            text.Span($"{_fletyCliente.FechaFactura}");
                        });

                    });
                   


                    row.RelativeItem().AlignCenter().Height(60).Width(100).Image(DamascoLogo);
                });
                col.Item().Text("Guía de despacho").AlignCenter().Bold().FontSize(12);
            });
        }

        void ComposeContent(IContainer container)
        {

            container.PaddingVertical(40).Column(column => {
                column.Spacing(20);
                column.Item().Element(DatosCliente);
                column.Item().Element(ProductosTable);
                column.Item().Text($"Observaciones: {_fletyCliente.Observaciones}").SemiBold();

            });
        
        
        }


        public void ProductosTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();

            container.Table(table => {

                table.ColumnsDefinition(columns =>
                {
                
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    //columns.RelativeColumn();
                });

                table.Header(header => {
                   
                    header.Cell().Text("SKU").Style(headerStyle);
                    header.Cell().Text("Descripción").Style(headerStyle);
                    header.Cell().Text("Cantidad").Style(headerStyle);
                    header.Cell().ColumnSpan(3).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);

                });

                foreach (var item in _fletyCliente.Articulos)
                {
                    table.Cell().Element(CellStyle).Text(item.CodArticulo);
                    table.Cell().Element(CellStyle).Text(item.Descripcion);

                    table.Cell().Element(CellStyle).Text(item.CantidadArticulo);
                  
               
              

                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);


                }
            });





        }


            public void DatosCliente(IContainer container)
        {
            container.ShowEntire().Column(column => {

                column.Spacing(2);
                column.Item().Text($"Cliente:{_fletyCliente.NomCliente}").SemiBold();
                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().PaddingBottom(5).LineHorizontal(1);

                column.Item().Text($"Tienda:{_fletyCliente.Sucursal}");
                column.Item().Text($"Dirección:{_fletyCliente.Direccion}");

                column.Item().Text($"Telefono:{_fletyCliente.NumeroTelefono}");
               
         




            });
        }
    }
}
