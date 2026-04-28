using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using VehicleManagementSystem.Entities;

namespace VehicleManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Parts> Parts { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<User> User{ get; set; }



    }
}
