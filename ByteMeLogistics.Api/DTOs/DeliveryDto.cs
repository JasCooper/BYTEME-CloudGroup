using ByteMeLogistics.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace ByteMeLogistics.Api.DTOs
{
    public class DeliveryDto
    {
        public int DeliveryId { get; set; }

        public string TrackingNumber { get; set; } = string.Empty;

        [Required]
        public string PackageDescription { get; set; } = string.Empty;

        [Required]
        public string PickupAddress { get; set; } = string.Empty;

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public int? DriverId { get; set; }

        public string? DriverName { get; set; }
    }
}