using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/customer-search")]
    [ApiController]
    public class CustomerSearchController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerSearchController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> SearchCustomerByVehicleId(Guid vehicleId)
        {
            var result = await dbContext.Vehicles
                .Where(v => v.VehicleId == vehicleId)
                .Join(
                    dbContext.Users,
                    vehicle => vehicle.OwnerId,
                    user => user.Id,
                    (vehicle, user) => new CustomerSearchResponseDto
                    {
                        CustomerId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        VehicleId = vehicle.VehicleId,
                        VehicleNumber = vehicle.VehicleNumber,
                        Brand = vehicle.Brand,
                        Model = vehicle.Model
                    }
                )
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return NotFound(new { message = "Customer not found for this vehicle ID." });
            }

            return Ok(result);
        }
    }
}