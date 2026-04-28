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

            return Ok(appointment);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetAppointments(string customerId)
        {
            var appointments = await dbContext.Appointments
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return Ok(appointments);
        }
    }
}