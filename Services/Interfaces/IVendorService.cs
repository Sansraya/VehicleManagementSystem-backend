using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Models;

namespace VehicleManagementSystem.Services.Interfaces
{
    public interface IVendorService
    {
        public Task<VendorResponse> AddPart(AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> UpdatePart(Guid partId, AddPartByVendorDTO vendorDTO);
        public Task<VendorResponse> DeletePart(Guid partId);
        Task<(bool Success, string Message, Guid? VendorId)> CreateVendorAsync(VendorModel model);
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto?> GetVendorByIdAsync(Guid id);
        Task<(bool Success, string Message)> UpdateVendorAsync(Guid id, VendorModel model);
        Task<(bool Success, string Message)> DeleteVendorAsync(Guid id);


        Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
            AssignVendorToPartRequestAsync(AssignVendorToPartRequestModel model, string assignedByUserId);

        Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
            AssignVendorToAppointmentAsync(AssignVendorToAppointmentModel model, string assignedByUserId);
        Task<IEnumerable<VendorAssignmentDto>> GetAllAssignmentsAsync();
        Task<IEnumerable<VendorAssignmentDto>> GetAssignmentsByPartRequestAsync(Guid partRequestId);
        Task<IEnumerable<VendorAssignmentDto>> GetAssignmentsByAppointmentAsync(Guid appointmentId);
        Task<VendorAssignmentDto?> GetAssignmentByIdAsync(Guid assignmentId);

        Task<(bool Success, string Message)> DeleteAssignmentAsync(Guid assignmentId);
        Task<IEnumerable<VendorAssignmentDto>> GetMyAssignmentsAsync(Guid vendorId);
        Task<VendorAssignmentDto?> GetMyAssignmentByIdAsync(Guid assignmentId, Guid vendorId);
        Task<(bool Success, string Message, VendorAssignmentDto? Assignment)>
         RespondToAssignmentAsync(Guid assignmentId, Guid vendorId, VendorRespondModel model);
        Task<(bool Success, string Message, VendorInvoiceDto? Invoice)>
         GenerateInvoiceAsync(Guid assignmentId, Guid vendorId, CreateVendorInvoiceModel model);

        Task<IEnumerable<VendorInvoiceDto>> GetMyInvoicesAsync(Guid vendorId);
        Task<VendorInvoiceDto?> GetMyInvoiceByIdAsync(Guid invoiceId, Guid vendorId);


    }
}
