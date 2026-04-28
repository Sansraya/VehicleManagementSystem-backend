using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<Customer> userManager;
        private readonly SignInManager<Customer> signInManager;

        public CustomerController(
            ApplicationDbContext dbContext,
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                UserName = model.Username,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth.HasValue
                    ? DateOnly.FromDateTime(model.DateOfBirth.Value)
                    : null,
                PhoneNumber = model.Phone
            };

            var result = await userManager.CreateAsync(customer, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                message = "Registration successful!",
                customerId = customer.Id,
                username = customer.UserName,
                email = customer.Email
            });
        }
    }
}