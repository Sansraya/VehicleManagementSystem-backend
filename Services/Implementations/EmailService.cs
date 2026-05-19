using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VehicleManagementSystem.Services.Interfaces;

namespace VehicleManagementSystem.Services.Implementations
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            var settings = configuration.GetSection("EmailSettings");
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings["FromName"], settings["FromEmail"]));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(settings["Host"], int.Parse(settings["Port"]!), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(settings["Username"], settings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendInvoiceEmailAsync(string toEmail, string toName, string invoiceNumber, DateTime saleDate, string paymentMethod, decimal totalAmount, decimal discountAmount, decimal finalAmount,
            IEnumerable<(string itemName, int qty, double unitPrice, double lineTotal)> items)
        {
            var itemRows = string.Join("", items.Select(i => $@"
                <tr>
                    <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;'>{i.itemName}</td>
                    <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:center;'>{i.qty}</td>
                    <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#374151;text-align:right;'>Rs. {i.unitPrice:N2}</td>
                    <td style='padding:10px 14px;border-bottom:1px solid #f3f4f6;color:#111827;font-weight:600;text-align:right;'>Rs. {i.lineTotal:N2}</td>
                </tr>"));

            var discountRow = discountAmount > 0
                ? $@"<tr>
                        <td colspan='3' style='padding:8px 14px;text-align:right;color:#6b7280;font-size:13px;'>Discount</td>
                        <td style='padding:8px 14px;text-align:right;color:#ef4444;font-weight:600;'>- Rs. {discountAmount:N2}</td>
                     </tr>"
                : "";

            var html = $@"

            <html lang='en'>
            <head><meta charset='UTF-8'/><meta name='viewport' content='width=device-width,initial-scale=1'/></head>
            <body style='margin:0;padding:0;background:#f9fafb;font-family:Inter,Arial,sans-serif;'>
              <div style='max-width:620px;margin:40px auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.07);'>

    
                <div style='background:#0f172a;padding:32px 36px;'>
                  <h1 style='margin:0;color:#fff;font-size:28px;font-weight:900;letter-spacing:-1px;'>
                    VMS<span style='color:#3b82f6;'>KTM</span>
                  </h1>
                  <p style='margin:6px 0 0;color:#94a3b8;font-size:13px;font-weight:600;letter-spacing:.05em;text-transform:uppercase;'>
                    Vehicle Management System
                  </p>
                </div>

    
                <div style='background:#eff6ff;padding:20px 36px;border-bottom:1px solid #dbeafe;display:flex;justify-content:space-between;align-items:center;'>
                  <div>
                    <p style='margin:0;color:#1d4ed8;font-size:11px;font-weight:800;letter-spacing:.12em;text-transform:uppercase;'>Invoice</p>
                    <p style='margin:4px 0 0;color:#1e3a8a;font-size:22px;font-weight:900;'>{invoiceNumber}</p>
                  </div>
                  <div style='text-align:right;'>
                    <p style='margin:0;color:#6b7280;font-size:12px;'>Date</p>
                    <p style='margin:4px 0 0;color:#111827;font-weight:700;font-size:14px;'>{saleDate:dd MMM yyyy}</p>
                  </div>
                </div>

    
                <div style='padding:28px 36px 0;'>
                  <p style='margin:0;color:#374151;font-size:15px;'>Hi <strong>{toName}</strong>,</p>
                  <p style='margin:8px 0 0;color:#6b7280;font-size:14px;line-height:1.6;'>
                    Thank you for your visit! Here is your invoice summary. Please keep this for your records.
                  </p>
                </div>

    
                <div style='padding:24px 36px;'>
                  <table style='width:100%;border-collapse:collapse;font-size:14px;'>
                    <thead>
                      <tr style='background:#f8fafc;'>
                        <th style='padding:10px 14px;text-align:left;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Item</th>
                        <th style='padding:10px 14px;text-align:center;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Qty</th>
                        <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Unit Price</th>
                        <th style='padding:10px 14px;text-align:right;color:#6b7280;font-size:11px;font-weight:700;letter-spacing:.08em;text-transform:uppercase;border-bottom:2px solid #e5e7eb;'>Total</th>
                      </tr>
                    </thead>
                    <tbody>
                      {itemRows}
                      {discountRow}
                      <tr style='background:#f8fafc;'>
                        <td colspan='3' style='padding:14px;text-align:right;color:#374151;font-weight:700;font-size:15px;'>Amount Due</td>
                        <td style='padding:14px;text-align:right;color:#1d4ed8;font-weight:900;font-size:20px;'>Rs. {finalAmount:N2}</td>
                      </tr>
                    </tbody>
                  </table>
                </div>

    
                <div style='margin:0 36px 28px;background:#f0fdf4;border:1px solid #bbf7d0;border-radius:10px;padding:14px 18px;display:flex;align-items:center;gap:10px;'>
                  <span style='color:#16a34a;font-size:18px;'>✓</span>
                  <p style='margin:0;color:#15803d;font-size:13px;font-weight:700;'>
                    Payment received via <strong>{paymentMethod}</strong>
                  </p>
                </div>

    
                <div style='background:#f8fafc;padding:24px 36px;border-top:1px solid #e5e7eb;'>
                  <p style='margin:0;color:#9ca3af;font-size:12px;line-height:1.6;'>
                    If you have any questions about this invoice, please contact us.<br/>
                    Thank you for choosing <strong>VMSKTM</strong> — we look forward to serving you again.
                  </p>
                </div>

              </div>
            </body>
            </html>";

            await SendEmailAsync(toEmail, toName, $"Your Invoice {invoiceNumber} — VMSKTM", html);
        }
    }
}