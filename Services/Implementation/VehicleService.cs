using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Data;
using VehicleManagementSystem.Dtos;
using VehicleManagementSystem.Dtos.Response;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Services.Implementation;

public class VehicleService : IVehicleService
{
    private readonly ApplicationDbContext dbContext;

    public VehicleService( ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<VehicleResponseDto>> GetAllVehicleAsync(
        bool? available, string? brand, string? fuelType, string? plateNumber)
    {
        var query = dbContext.Vehicles.AsQueryable();

        if (available.HasValue)
            query = query.Where(v => v.IsAvailable == available.Value);

        if (!string.IsNullOrEmpty(brand))
            query = query.Where(v => v.Brand.ToLower().Contains(brand.ToLower()));

        if (!string.IsNullOrEmpty(fuelType))
            query = query.Where(v => v.FuelType.ToLower() == fuelType.ToLower());

        if (!string.IsNullOrEmpty(plateNumber))
            query = query.Where(v => v.VehicleNumber.ToLower().Contains(plateNumber.ToLower()));

        return await query
            .OrderBy(v => v.Brand)
            .ThenBy(v => v.Model)
            .Select(v => ToDto(v))
            .ToListAsync();
    }

    public async Task<VehicleResponseDto?> GetVehicleByIdAsync(Guid id)
    {
        var vehicle = await dbContext.Vehicles.FindAsync(id);
        return vehicle is null ? null : ToDto(vehicle);
    }

    public async Task<(VehicleResponseDto vehicle, bool alreadyExists)> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var duplicate = await dbContext.Vehicles.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber);
        if (duplicate)
            return (null!, alreadyExists: true);

        var vehicle = new Vehicle
        {
            VehicleId         = Guid.NewGuid(),
            Brand             = dto.Brand,
            Model             = dto.Model,
            VehicleNumber     = dto.VehicleNumber,
            Colour            = dto.Colour,
            ManufacturingYear = dto.ManufacturingYear,
            FuelType          = dto.FuelType,
            Price             = dto.Price,
            IsAvailable       = dto.IsAvailable
        };

        dbContext.Vehicles.Add(vehicle);
        await dbContext.SaveChangesAsync();
        return (ToDto(vehicle), alreadyExists: false);
    }

    public async Task<(bool found, bool updated)> UpdateVehicleAsync(Guid id, UpdateVehicleDto dto)
    {
        var vehicle = await dbContext.Vehicles.FindAsync(id);
        if (vehicle is null)
            return (found: false, updated: false);

        if (dto.Brand       is not null) vehicle.Brand       = dto.Brand;
        if (dto.Model       is not null) vehicle.Model       = dto.Model;
        if (dto.Colour      is not null) vehicle.Colour      = dto.Colour;
        if (dto.FuelType    is not null) vehicle.FuelType    = dto.FuelType;
        if (dto.Price.HasValue)          vehicle.Price       = dto.Price.Value;
        if (dto.IsAvailable.HasValue)    vehicle.IsAvailable = dto.IsAvailable.Value;

        await dbContext.SaveChangesAsync();
        return (found: true, updated: true);
    }

    public async Task<(bool found, bool hasSales)> DeleteVehicleAsync(Guid id)
    {
        var vehicle = await dbContext.Vehicles.FindAsync(id);
        if (vehicle is null)
            return (found: false, hasSales: false);

        var wasSold = await dbContext.SaleItems.AnyAsync(si => si.VehicleId == id);
        if (wasSold)
            return (found: true, hasSales: true);

        dbContext.Vehicles.Remove(vehicle);
        await dbContext.SaveChangesAsync();
        return (found: true, hasSales: false);
    }
    
    private static VehicleResponseDto ToDto(Vehicle v) => new()
    {
        VehicleId         = v.VehicleId,
        Brand             = v.Brand,
        Model             = v.Model,
        VehicleNumber     = v.VehicleNumber,
        Colour            = v.Colour,
        ManufacturingYear = v.ManufacturingYear,
        FuelType          = v.FuelType,
        Price             = v.Price,
        IsAvailable       = v.IsAvailable
    };
}