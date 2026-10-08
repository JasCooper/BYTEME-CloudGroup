using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.DTOs
{
    public class DriverDto
    {
        public int DriverId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        public string VehicleRegistration { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
    }
}