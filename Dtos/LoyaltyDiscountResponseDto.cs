namespace VehicleManagementSystem.Dtos
{
    public class LoyaltyDiscountResponseDto
    {
        public decimal TotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public bool IsEligible { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}