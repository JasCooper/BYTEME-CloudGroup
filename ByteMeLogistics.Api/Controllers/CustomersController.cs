using ByteMeLogistics.Api.Data;
using ByteMeLogistics.Api.DTOs;
using ByteMeLogistics.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ByteMeLogistics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var customers = await _context.Customers
                .OrderBy(c => c.Name)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address
                })
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address
                })
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(
            CustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            dto.CustomerId = customer.CustomerId;

            return CreatedAtAction(
                nameof(GetCustomer),
                new { id = customer.CustomerId },
                dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            CustomerDto dto)
        {
            var customer =
                await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer =
                await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            bool hasDeliveries =
                await _context.Deliveries.AnyAsync(
                    d => d.CustomerId == id);

            if (hasDeliveries)
            {
                return BadRequest(
                    "Customer cannot be deleted because deliveries are linked to this customer.");
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}