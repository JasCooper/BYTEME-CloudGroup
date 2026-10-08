using ByteMeLogistics.Api.Data;
using ByteMeLogistics.Api.DTOs;
using ByteMeLogistics.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteMeLogistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriverDto>>> GetDrivers()
        {
            var drivers = await _context.Drivers
                .OrderBy(d => d.Name)
                .Select(d => new DriverDto
                {
                    DriverId = d.DriverId,
                    Name = d.Name,
                    PhoneNumber = d.PhoneNumber,
                    VehicleType = d.VehicleType,
                    VehicleRegistration = d.VehicleRegistration,
                    IsAvailable = d.IsAvailable
                })
                .ToListAsync();

            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDto>> GetDriver(int id)
        {
            var driver = await _context.Drivers
                .Where(d => d.DriverId == id)
                .Select(d => new DriverDto
                {
                    DriverId = d.DriverId,
                    Name = d.Name,
                    PhoneNumber = d.PhoneNumber,
                    VehicleType = d.VehicleType,
                    VehicleRegistration = d.VehicleRegistration,
                    IsAvailable = d.IsAvailable
                })
                .FirstOrDefaultAsync();

            if (driver == null)
            {
                return NotFound();
            }

            return Ok(driver);
        }

        [HttpPost]
        public async Task<ActionResult<DriverDto>> CreateDriver(
            DriverDto dto)
        {
            var driver = new Driver
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                VehicleType = dto.VehicleType,
                VehicleRegistration = dto.VehicleRegistration,
                IsAvailable = dto.IsAvailable
            };

            _context.Drivers.Add(driver);

            await _context.SaveChangesAsync();

            dto.DriverId = driver.DriverId;

            return CreatedAtAction(
                nameof(GetDriver),
                new { id = driver.DriverId },
                dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriver(
            int id,
            DriverDto dto)
        {
            var driver =
                await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            driver.Name = dto.Name;
            driver.PhoneNumber = dto.PhoneNumber;
            driver.VehicleType = dto.VehicleType;
            driver.VehicleRegistration =
                dto.VehicleRegistration;

            driver.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            var driver =
                await _context.Drivers.FindAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            var deliveries = await _context.Deliveries
                .Where(d => d.DriverId == id)
                .ToListAsync();

            foreach (var delivery in deliveries)
            {
                delivery.DriverId = null;

                if (delivery.Status ==
                    DeliveryStatus.Assigned)
                {
                    delivery.Status =
                        DeliveryStatus.Pending;
                }
            }

            _context.Drivers.Remove(driver);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}