using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public AppointmentController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentDto dto)
        {
            var customerExists = await dbContext.Users.AnyAsync(u => u.Id == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(new { message = "Invalid customer ID." });
            }

            var appointment = new Appointment
            {
                CustomerId = dto.CustomerId,
                AppointmentDate = dto.AppointmentDate.ToUniversalTime(),
                ServiceType = dto.ServiceType,
                Description = dto.Description
            };

            dbContext.Appointments.Add(appointment);
            await dbContext.SaveChangesAsync();

            var response = new AppointmentResponseDto
            {
                AppointmentId = appointment.AppointmentId,
                CustomerId = appointment.CustomerId,
                AppointmentDate = appointment.AppointmentDate,
                ServiceType = appointment.ServiceType,
                Description = appointment.Description
            };

            return Ok(response);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAppointments(string customerId)
        {
            var appointments = await dbContext.Appointments
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentResponseDto
                {
                    AppointmentId = a.AppointmentId,
                    CustomerId = a.CustomerId,
                    AppointmentDate = a.AppointmentDate,
                    ServiceType = a.ServiceType,
                    Description = a.Description
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}