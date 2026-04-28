using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<Customer>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PartRequest> PartRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
