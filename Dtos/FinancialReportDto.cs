namespace VehicleManagementSystem.Dtos
{
    public class FinancialReportDto
    {
        public string Period { get; set; } = string.Empty; 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal TotalPurchaseCost { get; set; }
        public decimal GrossProfit { get; set; }

        public int TotalSales { get; set; }
        public int CompletedSales { get; set; }
        public int PendingSales { get; set; }

        public int TotalPurchaseInvoices { get; set; }
        public decimal OutstandingPayables { get; set; }

        public List<RevenueBreakdownDto> Breakdown { get; set; } = new();
        public List<PaymentMethodBreakdownDto> PaymentMethods { get; set; } = new();
    }

    public class RevenueBreakdownDto
    {
        public string Label { get; set; } = string.Empty; 
        public decimal Revenue { get; set; }
        public decimal Discount { get; set; }
        public decimal NetRevenue { get; set; }
        public int SaleCount { get; set; }
    }

    public class PaymentMethodBreakdownDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}