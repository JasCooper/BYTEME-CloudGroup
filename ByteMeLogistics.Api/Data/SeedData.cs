using ByteMeLogistics.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ByteMeLogistics.Api.Data
{
    public static class SeedData
    {
        public static async Task InitialiseAsync(
            ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            if (await context.Customers.AnyAsync())
            {
                return;
            }

            var customers = new List<Customer>
            {
                new Customer
                {
                    Name = "Thabo Mkhize",
                    Email = "thabo@email.com",
                    PhoneNumber = "0825550192",
                    Address = "Umhlanga, Durban"
                },

                new Customer
                {
                    Name = "Sarah Naidoo",
                    Email = "sarah@email.com",
                    PhoneNumber = "0834428810",
                    Address = "Westville, Durban"
                },

                new Customer
                {
                    Name = "Michael Dlamini",
                    Email = "michael@email.com",
                    PhoneNumber = "0729001122",
                    Address = "Pinetown, Durban"
                }
            };

            context.Customers.AddRange(customers);

            var drivers = new List<Driver>
            {
                new Driver
                {
                    Name = "Sipho Khumalo",
                    PhoneNumber = "0821112233",
                    VehicleType = "Toyota Quantum",
                    VehicleRegistration = "ND 458 921",
                    IsAvailable = true
                },

                new Driver
                {
                    Name = "Ryan Pillay",
                    PhoneNumber = "0832223344",
                    VehicleType = "Ford Transit",
                    VehicleRegistration = "ND 783 210",
                    IsAvailable = true
                },

                new Driver
                {
                    Name = "Lwazi Mthembu",
                    PhoneNumber = "0843334455",
                    VehicleType = "Toyota Hilux",
                    VehicleRegistration = "ND 224 198",
                    IsAvailable = false
                }
            };

            context.Drivers.AddRange(drivers);

            await context.SaveChangesAsync();

            var deliveries = new List<Delivery>
            {
                new Delivery
                {
                    TrackingNumber = "BYTE-1001",
                    PackageDescription = "Electronics Package",
                    PickupAddress = "Durban CBD",
                    DeliveryAddress = "Umhlanga Rocks",
                    CustomerId = customers[0].CustomerId,
                    DriverId = drivers[0].DriverId,
                    Status = DeliveryStatus.InTransit,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    ExpectedDeliveryDate =
                        DateTime.UtcNow.AddDays(1)
                },

                new Delivery
                {
                    TrackingNumber = "BYTE-1002",
                    PackageDescription = "Office Supplies",
                    PickupAddress = "Springfield Park",
                    DeliveryAddress = "Westville",
                    CustomerId = customers[1].CustomerId,
                    DriverId = drivers[1].DriverId,
                    Status = DeliveryStatus.Assigned,
                    CreatedDate = DateTime.UtcNow,
                    ExpectedDeliveryDate =
                        DateTime.UtcNow.AddDays(2)
                },

                new Delivery
                {
                    TrackingNumber = "BYTE-1003",
                    PackageDescription = "Business Documents",
                    PickupAddress = "La Lucia",
                    DeliveryAddress = "Pinetown",
                    CustomerId = customers[2].CustomerId,
                    DriverId = drivers[2].DriverId,
                    Status = DeliveryStatus.Delivered,
                    CreatedDate = DateTime.UtcNow.AddDays(-3),
                    ExpectedDeliveryDate =
                        DateTime.UtcNow.AddDays(-1)
                }
            };

            context.Deliveries.AddRange(deliveries);

            await context.SaveChangesAsync();
        }
    }
}