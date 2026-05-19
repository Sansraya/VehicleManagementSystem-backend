namespace VehicleManagementSystem.Dtos
{
    public class CustomerReportSummaryDto
    {
        public int TotalCustomers { get; set; }
        public int RegularCustomers { get; set; }      
        public int HighSpenders { get; set; }         
        public int PendingCreditsCount { get; set; }
        public decimal PendingCreditsAmount { get; set; }
    }

    public class RegularCustomerDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int VisitCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastVisit { get; set; }
    }

    public class HighSpenderDto
    {
        public string CustomerId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal TotalSpent { get; set; }
        public int PurchaseCount { get; set; }
        public decimal AverageSpend { get; set; }
        public DateTime? LastVisit { get; set; }
    }

    public class PendingCreditDto
    {
        public Guid SaleId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int DaysOutstanding { get; set; }
    }
}