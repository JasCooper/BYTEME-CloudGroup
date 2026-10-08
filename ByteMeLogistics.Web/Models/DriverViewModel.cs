using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Web.Models
{
    public class DriverViewModel
    {
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "Driver Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Registration")]
        public string VehicleRegistration { get; set; }
            = string.Empty;

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true;
    }
}