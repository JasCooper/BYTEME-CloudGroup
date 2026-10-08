using ByteMeLogistics.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ByteMeLogistics.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Deliveries)
                .WithOne(d => d.Customer)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.Deliveries)
                .WithOne(d => d.Driver)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Delivery>()
                .HasIndex(d => d.TrackingNumber)
                .IsUnique();
        }
    }
}