using CsvHelper;
using Loanity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Loanity.Domain.IExports
{
    public class LoanExportService : IExportService<Loan>
    {
        public byte[] ExportToCsv(IEnumerable<Loan> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(data);
            writer.Flush();

            return memoryStream.ToArray();
        }

        public byte[] ExportToPdf(IEnumerable<Loan> data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("Loans Report").SemiBold().FontSize(20).FontColor(Colors.Green.Darken2);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // Id
                            columns.RelativeColumn(); // UserId
                            columns.RelativeColumn(); // StartAt
                            columns.RelativeColumn(); // DueAt
                            columns.RelativeColumn(); // ReturnedAt
                            columns.RelativeColumn(); // Status
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("Id").Bold();
                            header.Cell().Text("UserId").Bold();
                            header.Cell().Text("Start At").Bold();
                            header.Cell().Text("Due At").Bold();
                            header.Cell().Text("Returned At").Bold();
                            header.Cell().Text("Status").Bold();
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Text(item.Id.ToString() ?? "-");
                            table.Cell().Text(item.UserId.ToString() ?? "-");
                            table.Cell().Text(item.StartAt.ToString("dd.MM.yyyy HH:mm") ?? "-");
                            table.Cell().Text(item.DueAt.ToString("dd.MM.yyyy HH:mm") ?? "-");
                            table.Cell().Text(item.ReturnedAt?.ToString("dd.MM.yyyy HH:mm") ?? "-");
                            table.Cell().Text(item.Status.ToString() ?? "-");
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
