using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public ICollection<Delivery> Deliveries { get; set; }
            = new List<Delivery>();
    }
}