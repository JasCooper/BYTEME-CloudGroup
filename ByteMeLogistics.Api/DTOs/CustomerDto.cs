using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.DTOs
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}