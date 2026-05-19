namespace VehicleManagementSystem.Dtos
{
   

    public class VendorUserRegisterModel
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        public Guid VendorId { get; set; }
    }

    public class VendorUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Guid VendorId { get; set; }
        public string? VendorName { get; set; }
    }

    
    public class AssignVendorToPartRequestModel
    {
        public Guid PartRequestId { get; set; }
        public Guid VendorId { get; set; }
        public string? Notes { get; set; }
    }

    
    public class AssignVendorToAppointmentModel
    {
        public Guid AppointmentId { get; set; }
        public Guid VendorId { get; set; }
        public string? Notes { get; set; }
    }

    
    public class VendorAssignmentDto
    {
        public Guid VendorAssignmentId { get; set; }
        public Guid VendorId { get; set; }
        public string? VendorName { get; set; }

        public Guid? PartRequestId { get; set; }
        public string? PartRequestPartName { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? AppointmentServiceType { get; set; }

        public string AssignedByUserId { get; set; } = string.Empty;
        public string? AssignedByName { get; set; }
        public DateTime AssignedAt { get; set; }

        
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string? VendorNotes { get; set; }
       
        public Guid? VendorInvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
    }

    
    public class VendorRespondModel
    {
        
        public required string Decision { get; set; }
        public string? VendorNotes { get; set; }
    }

    
    public class CreateVendorInvoiceModel
    {
        public string? Notes { get; set; }
        public required List<VendorInvoiceItemModel> Items { get; set; }
    }

    public class VendorInvoiceItemModel
    {
        public required string Description { get; set; }
        
        public string ItemType { get; set; } = "Service";
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }

   
    public class VendorInvoiceDto
    {
        public Guid VendorInvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid VendorId { get; set; }
        public string? VendorName { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public Guid VendorAssignmentId { get; set; }

        
        public Guid? PartRequestId { get; set; }
        public string? PartRequestPartName { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? AppointmentServiceType { get; set; }

        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime IssuedAt { get; set; }

        public List<VendorInvoiceItemDto> Items { get; set; } = new();
    }

    public class VendorInvoiceItemDto
    {
        public Guid VendorInvoiceItemId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    
    public class UpdateVendorInvoiceStatusModel
    {
    
        public required string Status { get; set; }
    }
}