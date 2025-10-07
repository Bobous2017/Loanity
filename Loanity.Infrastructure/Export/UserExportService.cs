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
    public class UserExportService : IExportService<User>
    {
        public byte[] ExportToCsv(IEnumerable<User> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(data);
            writer.Flush();

            return memoryStream.ToArray();
        }


        public byte[] ExportToPdf(IEnumerable<User> data)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text("Users Report").SemiBold().FontSize(20).FontColor(Colors.Red.Darken2);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // Id
                            columns.RelativeColumn(); // FirstName
                            columns.RelativeColumn(); // LastName
                            columns.RelativeColumn(); // Email
                            columns.RelativeColumn(); // Phone
                            columns.RelativeColumn(); // Role
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("Id").Bold();
                            header.Cell().Text("First Name").Bold();
                            header.Cell().Text("Last Name").Bold();
                            header.Cell().Text("Email").Bold();
                            header.Cell().Text("Phone").Bold();
                            header.Cell().Text("Role").Bold();
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Text(item.Id.ToString());
                            table.Cell().Text(item.FirstName ?? "-");
                            table.Cell().Text(item.LastName ?? "-");
                            table.Cell().Text(item.Email ?? "-");
                            table.Cell().Text(item.Phone ?? "-");
                            table.Cell().Text(item.Role?.Name ?? "-");



                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
