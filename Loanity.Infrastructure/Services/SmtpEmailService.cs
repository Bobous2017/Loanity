using Loanity.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Infrastructure.Services
{

    public class SmtpEmailService : IEmailService
    {
        public async Task SendAsync(string to, string subject, string body, Stream? attachment = null, string? attachmentName = null)
        {
            using var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("bobousenselemani2017@gmail.com", "dmfs cdca gmzm xiuy"), // ⚠️ move to appsettings.json later
                EnableSsl = true
            };

            var mail = new MailMessage("bobousenselemani2017@gmail.com", to, subject, body)
            {
                IsBodyHtml = true
            };

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                mail.Attachments.Add(new Attachment(attachment, attachmentName, "image/png"));
            }

            await client.SendMailAsync(mail);
        }
    }

}
