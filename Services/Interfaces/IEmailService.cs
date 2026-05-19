namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
        Task SendInvoiceEmailAsync(string toEmail, string toName, string invoiceNumber,
            DateTime saleDate, string paymentMethod, decimal totalAmount,
            decimal discountAmount, decimal finalAmount,
            IEnumerable<(string itemName, int qty, double unitPrice, double lineTotal)> items);
        Task SendCreditReminderEmailAsync(string toEmail, string toName, string invoiceNumber, DateTime saleDate, decimal amountDue, int daysOverdue);
    }
}