using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class GenerateQRCode
    {
        // Updated: userId is now optional (nullable) and we always return a MemoryStream
        public MemoryStream GenerateQRCodeGen(string qrText, int? userId, SKBitmap? qrCodeBitmap = null)
        {
            if (string.IsNullOrWhiteSpace(qrText))
                throw new ArgumentException("qrText is required", nameof(qrText));

            // Build payload (include user id if provided)
            var payload = userId.HasValue ? $"{qrText}-{userId.Value}" : qrText;

            // Ensure bitmap instance
            qrCodeBitmap ??= new SKBitmap(300, 300);

            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 250,
                    Height = 250
                }
            };

            var pixelData = qrCodeWriter.Write(payload);
            var stream = new MemoryStream();

            // Create bitmap from pixel data
            qrCodeBitmap = new SKBitmap(pixelData.Width, pixelData.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            Marshal.Copy(pixelData.Pixels, 0, qrCodeBitmap.GetPixels(), pixelData.Pixels.Length);

            // Encode to PNG
            qrCodeBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Position = 0;
            return stream; // Never return null now
        }

        public async Task SendEmailWithQRCode(
            string toEmail,
            string equipmentName,
            string pickupLocation,
            DateTime startAt,
            DateTime endAt,
            string qrCodeBase64,
            string reservationCode)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential(
                    "bobousenselemani2017@gmail.com",
                    "ttjj fiwq pgfj kvvr"
                ),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("bobousenselemani2017@gmail.com"),
                Subject = $"Reservation Confirmation – {equipmentName}",
                Body = $@"
                        <p>Hello,</p>
                        <p>Your reservation has been confirmed.</p>

                        <p><b>Reservation Code:</b> {reservationCode}</p>
                        <p><b>Equipment:</b> {equipmentName}</p>
                        <p><b>Pickup Location:</b> {pickupLocation}</p>
                        <p><b>Start:</b> {startAt:G}</p>
                        <p><b>End:</b> {endAt:G}</p>

                        <p>Please present the attached QR code when you come to pick up your equipment.</p>
                        <p>Thank you,<br/>Loanity Team</p>
                    ",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            if (!string.IsNullOrEmpty(qrCodeBase64))
            {
                byte[] qrCodeBytes = Convert.FromBase64String(qrCodeBase64);
                using var ms = new MemoryStream(qrCodeBytes);
                var attachment = new Attachment(ms, "reservation_qrcode.png", "image/png");
                mailMessage.Attachments.Add(attachment);
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}