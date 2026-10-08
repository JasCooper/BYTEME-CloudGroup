using ByteMeLogistics.Api.Data;
using ByteMeLogistics.Api.DTOs;
using ByteMeLogistics.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteMeLogistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DeliveriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryDto>>>
            GetDeliveries(
                string? search = null,
                DeliveryStatus? status = null)
        {
            var query = _context.Deliveries
                .Include(d => d.Customer)
                .Include(d => d.Driver)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.TrackingNumber.Contains(search) ||
                    d.PackageDescription.Contains(search) ||
                    d.Customer!.Name.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(
                    d => d.Status == status.Value);
            }

            var deliveries = await query
                .OrderByDescending(d => d.CreatedDate)
                .Select(d => new DeliveryDto
                {
                    DeliveryId = d.DeliveryId,
                    TrackingNumber = d.TrackingNumber,
                    PackageDescription =
                        d.PackageDescription,
                    PickupAddress = d.PickupAddress,
                    DeliveryAddress =
                        d.DeliveryAddress,
                    CreatedDate = d.CreatedDate,
                    ExpectedDeliveryDate =
                        d.ExpectedDeliveryDate,
                    Status = d.Status,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer!.Name,
                    DriverId = d.DriverId,
                    DriverName = d.Driver != null
                        ? d.Driver.Name
                        : null
                })
                .ToListAsync();

            return Ok(deliveries);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryDto>>
            GetDelivery(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Customer)
                .Include(d => d.Driver)
                .Where(d => d.DeliveryId == id)
                .Select(d => new DeliveryDto
                {
                    DeliveryId = d.DeliveryId,
                    TrackingNumber = d.TrackingNumber,
                    PackageDescription =
                        d.PackageDescription,
                    PickupAddress = d.PickupAddress,
                    DeliveryAddress =
                        d.DeliveryAddress,
                    CreatedDate = d.CreatedDate,
                    ExpectedDeliveryDate =
                        d.ExpectedDeliveryDate,
                    Status = d.Status,
                    CustomerId = d.CustomerId,
                    CustomerName = d.Customer!.Name,
                    DriverId = d.DriverId,
                    DriverName = d.Driver != null
                        ? d.Driver.Name
                        : null
                })
                .FirstOrDefaultAsync();

            if (delivery == null)
            {
                return NotFound();
            }

            return Ok(delivery);
        }

        [HttpPost]
        public async Task<ActionResult<DeliveryDto>>
            CreateDelivery(DeliveryDto dto)
        {
            bool customerExists =
                await _context.Customers.AnyAsync(
                    c => c.CustomerId == dto.CustomerId);

            if (!customerExists)
            {
                return BadRequest(
                    "The selected customer does not exist.");
            }

            if (dto.DriverId.HasValue)
            {
                bool driverExists =
                    await _context.Drivers.AnyAsync(
                        d => d.DriverId == dto.DriverId);

                if (!driverExists)
                {
                    return BadRequest(
                        "The selected driver does not exist.");
                }
            }

            var delivery = new Delivery
            {
                TrackingNumber =
                    $"BYTE-{DateTime.UtcNow:yyyyMMddHHmmssfff}",

                PackageDescription =
                    dto.PackageDescription,

                PickupAddress =
                    dto.PickupAddress,

                DeliveryAddress =
                    dto.DeliveryAddress,

                CustomerId =
                    dto.CustomerId,

                DriverId =
                    dto.DriverId,

                ExpectedDeliveryDate =
                    dto.ExpectedDeliveryDate,

                CreatedDate =
                    DateTime.UtcNow,

                Status = dto.DriverId.HasValue
                    ? DeliveryStatus.Assigned
                    : DeliveryStatus.Pending
            };

            _context.Deliveries.Add(delivery);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDelivery),
                new { id = delivery.DeliveryId },
                new
                {
                    delivery.DeliveryId,
                    delivery.TrackingNumber
                });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDelivery(
            int id,
            DeliveryDto dto)
        {
            var delivery =
                await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            delivery.PackageDescription =
                dto.PackageDescription;

            delivery.PickupAddress =
                dto.PickupAddress;

            delivery.DeliveryAddress =
                dto.DeliveryAddress;

            delivery.ExpectedDeliveryDate =
                dto.ExpectedDeliveryDate;

            delivery.CustomerId =
                dto.CustomerId;

            delivery.DriverId =
                dto.DriverId;

            delivery.Status =
                dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            DeliveryStatus status)
        {
            var delivery =
                await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            delivery.Status = status;

            if (delivery.DriverId.HasValue)
            {
                var driver =
                    await _context.Drivers.FindAsync(
                        delivery.DriverId);

                if (driver != null)
                {
                    if (status ==
                        DeliveryStatus.Delivered ||
                        status ==
                        DeliveryStatus.Cancelled)
                    {
                        driver.IsAvailable = true;
                    }
                    else if (status ==
                             DeliveryStatus.Assigned ||
                             status ==
                             DeliveryStatus.InTransit)
                    {
                        driver.IsAvailable = false;
                    }
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            var delivery =
                await _context.Deliveries.FindAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            _context.Deliveries.Remove(delivery);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}