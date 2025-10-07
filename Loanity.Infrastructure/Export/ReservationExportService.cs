using CsvHelper;
using Loanity.Domain.Entities;
using Loanity.Domain.IExports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Loanity.Infrastructure.Export
{
    public class ReservationExportService : IExportService<Reservation>
    {
        public byte[] ExportToCsv(IEnumerable<Reservation> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(data);
            writer.Flush();

            return memoryStream.ToArray();
        }

        public byte[] ExportToPdf(IEnumerable<Reservation> data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("Reservations Report").SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // Id
                            columns.RelativeColumn(); // UserId
                            columns.RelativeColumn(); // EquipmentId
                            columns.RelativeColumn(); // Start
                            columns.RelativeColumn(); // End
                            columns.RelativeColumn(); // Status
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("Id").Bold();
                            header.Cell().Text("UserId").Bold();
                            header.Cell().Text("EquipmentId").Bold();
                            header.Cell().Text("Start At").Bold();
                            header.Cell().Text("End At").Bold();
                            header.Cell().Text("Status").Bold();
                        });

                        // Data
                        foreach (var item in data)
                        {
                            table.Cell().Text(item.Id.ToString());
                            table.Cell().Text(item.UserId.ToString() ?? "-");
                            table.Cell().Text(item.EquipmentId.ToString() ?? "-");
                            table.Cell().Text(item.StartAt.ToString("dd.MM.yyyy HH:mm"));
                            table.Cell().Text(item.EndAt.ToString("dd.MM.yyyy HH:mm"));
                            table.Cell().Text(item.Status.ToString() ?? "-");

                        }
                    });
                });
            });

            return document.GeneratePdf();
        }


    }
}
