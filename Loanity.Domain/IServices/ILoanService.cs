using Loanity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IServices
{
    public interface ILoanService
    {
        //Loan Business rules  ( loaning/returning ) ----------------------------------1
        Task<Loan> CreateLoanFromScanAsync(int userId, string qrCode, DateTime dueAt);
        Task<Loan?> ReturnByScanAsync(int userId, string qrCode);

    }
}
