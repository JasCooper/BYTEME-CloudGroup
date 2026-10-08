using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.Models
{
    public class Delivery
    {
        public int DeliveryId { get; set; }

        [Required]
        [StringLength(50)]
        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PackageDescription { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string PickupAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ExpectedDeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; }
            = DeliveryStatus.Pending;

        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        public int? DriverId { get; set; }

        public Driver? Driver { get; set; }
    }
}