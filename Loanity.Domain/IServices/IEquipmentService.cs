using Loanity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IServices
{
    public interface IEquipmentService
    {
        Task<Equipment?> GetByQrAsync(string qr);

    }
}
