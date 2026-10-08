using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Web.Models
{
    public class DeliveryViewModel
    {
        public int DeliveryId { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Package Description")]
        public string PackageDescription { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Pickup Address")]
        public string PickupAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expected Delivery")]
        public DateTime ExpectedDeliveryDate { get; set; }
            = DateTime.Today.AddDays(2);

        public DeliveryStatus Status { get; set; }

        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        [Display(Name = "Driver")]
        public int? DriverId { get; set; }

        public string? DriverName { get; set; }
    }
}