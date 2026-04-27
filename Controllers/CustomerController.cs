using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Entities;
using VehicleManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
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


        public CustomerController(ApplicationDbContext dbContext, UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }


        [HttpPost("register")]

        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            var alreadyEmail = await userManager.FindByEmailAsync(model.Email);
            var alreadyUsername = await userManager.FindByNameAsync(model.Username);

            if (alreadyUsername != null)
            {
                return BadRequest(new { message = "Username Already Exists! Please Try Again!" });

            }

            if (alreadyEmail != null)
            {
                return BadRequest(new { message = "Email Already Exists! Please Try Again!" });

            }

            var customer = new Customer
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.HasValue
                    ? DateOnly.FromDateTime(model.DateOfBirth.Value)
                    : default,
                Address = model.Address,
                PhoneNumber = model.Phone,

            };
            var result = await userManager.CreateAsync(customer, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(customer, "Customer");
            await dbContext.SaveChangesAsync();
            return Ok(new { message = "Registration successful!" });

        }
    }
}
