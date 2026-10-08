namespace ByteMeLogistics.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalDeliveries { get; set; }

        public int PendingDeliveries { get; set; }

        public int AssignedDeliveries { get; set; }

        public int InTransitDeliveries { get; set; }

        public int DeliveredDeliveries { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalDrivers { get; set; }

        public int AvailableDrivers { get; set; }

        public List<DeliveryViewModel> RecentDeliveries { get; set; }
            = new();
    }
}