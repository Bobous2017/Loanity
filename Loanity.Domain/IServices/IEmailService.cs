using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.IServices
{
    public interface IEmailService
    {
      
            Task SendAsync(string to, string subject, string body, Stream? attachment = null, string? attachmentName = null);
        

    }

}
