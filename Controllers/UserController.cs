using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos.Models;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;

    public CustomerController(
        ApplicationDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterModel model)
    {
        if (await userManager.FindByNameAsync(model.Username) != null)
            return BadRequest(new { message = "Username Already Exists! Please Try Again!" });

        if (await userManager.FindByEmailAsync(model.Email) != null)
            return BadRequest(new { message = "Email Already Exists! Please Try Again!" });

        var customer = new User
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

   
    [HttpPost("register-with-vehicle")]
    public async Task<IActionResult> RegisterWithVehicle(
        [FromBody] RegisterCustomerWithVehicleModel model)
    {
  
        if (await userManager.FindByNameAsync(model.Username) != null)
            return BadRequest(new { message = "Username already exists." });

        if (await userManager.FindByEmailAsync(model.Email) != null)
            return BadRequest(new { message = "Email already registered." });

        
        if (model.Vehicle is not null)
        {
            var plateExists = await dbContext.Vehicles
                .AnyAsync(v => v.VehicleNumber == model.Vehicle.VehicleNumber);

            if (plateExists)
                return Conflict(new
                {
                    message = $"A vehicle with plate '{model.Vehicle.VehicleNumber}' already exists."
                });
        }

        
        var customer = new User
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
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(customer, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = errors });
        }

        await userManager.AddToRoleAsync(customer, "Customer");

        
        Guid? vehicleId = null;
        if (model.Vehicle is not null)
        {
            var vehicle = new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                Brand = model.Vehicle.Brand,
                Model = model.Vehicle.Model,
                VehicleNumber = model.Vehicle.VehicleNumber,
                Colour = model.Vehicle.Colour,
                ManufacturingYear = model.Vehicle.ManufacturingYear,
                FuelType = model.Vehicle.FuelType,
                Price = model.Vehicle.Price,
                IsAvailable = false,   
                OwnerId = customer.Id,
            };

            dbContext.Vehicles.Add(vehicle);
            vehicleId = vehicle.VehicleId;
        }

        await dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = model.Vehicle is null
                ? "Customer registered successfully."
                : "Customer registered with vehicle.",
            customerId = customer.Id,
            vehicleId
        });
    }

    [HttpPost("register-only-vehicle")]
    public async Task<IActionResult> RegisterOnlyVehicle(
        [FromBody] RegisterCustomerWithVehicleModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (model.Vehicle is not null)
        {
            var plateExists = await dbContext.Vehicles
                .AnyAsync(v => v.VehicleNumber == model.Vehicle.VehicleNumber);

            if (plateExists)
                return Conflict(new
                {
                    message = $"A vehicle with plate '{model.Vehicle.VehicleNumber}' already exists."
                });
        }


        Guid? vehicleId = null;
        if (model.Vehicle is not null)
        {
            var vehicle = new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                Brand = model.Vehicle.Brand,
                Model = model.Vehicle.Model,
                VehicleNumber = model.Vehicle.VehicleNumber,
                Colour = model.Vehicle.Colour,
                ManufacturingYear = model.Vehicle.ManufacturingYear,
                FuelType = model.Vehicle.FuelType,
                Price = model.Vehicle.Price,
                IsAvailable = false,
                OwnerId = user.Id,
            };

            dbContext.Vehicles.Add(vehicle);
            vehicleId = vehicle.VehicleId;
        }

        await dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = model.Vehicle is null
                ? "Customer registered successfully."
                : "Customer registered with vehicle.",
            customerId = user.Id,
            vehicleId
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await userManager.GetUsersInRoleAsync("Customer");

        
        var customerIds = customers.Select(c => c.Id).ToList();
        var vehicles = await dbContext.Vehicles
            .Where(v => v.OwnerId != null && customerIds.Contains(v.OwnerId!))
            .ToListAsync();

        var vehicleMap = vehicles.ToDictionary(v => v.OwnerId!, v => v);

        var result = customers.Select(u =>
        {
            vehicleMap.TryGetValue(u.Id, out var v);
            return new
            {
                id = u.Id,
                userName = u.UserName,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                phoneNumber = u.PhoneNumber,
                address = u.Address,
                vehicle = v == null ? null : new
                {
                    vehicleId = v.VehicleId,
                    brand = v.Brand,
                    model = v.Model,
                    vehicleNumber = v.VehicleNumber,
                    colour = v.Colour,
                    manufacturingYear = v.ManufacturingYear,
                    fuelType = v.FuelType,
                    price = v.Price,
                }
            };
        });

        return Ok(result);
    }
}