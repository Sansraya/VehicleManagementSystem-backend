namespace VehicleManagementSystem.Dtos;

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
}