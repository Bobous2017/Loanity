using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IExports
{
    public interface IExportService<T> // Accepts a generic type  for reservations, equipment, loan, users, etc.
    {
        byte[] ExportToCsv(IEnumerable<T> data); // csv export
        byte[] ExportToPdf(IEnumerable<T> data); // pdf export
    }

}
