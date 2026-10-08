using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.Models
{
    public class Driver
    {
        public int DriverId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string VehicleRegistration { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public ICollection<Delivery> Deliveries { get; set; }
            = new List<Delivery>();
    }
}