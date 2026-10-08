using ByteMeLogistics.Web.Models;
using ByteMeLogistics.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ByteMeLogistics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var deliveries =
                await _apiService.GetDeliveriesAsync();

            var customers =
                await _apiService.GetCustomersAsync();

            var drivers =
                await _apiService.GetDriversAsync();

            var model = new DashboardViewModel
            {
                TotalDeliveries =
                    deliveries.Count,

                PendingDeliveries =
                    deliveries.Count(d =>
                        d.Status ==
                        DeliveryStatus.Pending),

                AssignedDeliveries =
                    deliveries.Count(d =>
                        d.Status ==
                        DeliveryStatus.Assigned),

                InTransitDeliveries =
                    deliveries.Count(d =>
                        d.Status ==
                        DeliveryStatus.InTransit),

                DeliveredDeliveries =
                    deliveries.Count(d =>
                        d.Status ==
                        DeliveryStatus.Delivered),

                TotalCustomers =
                    customers.Count,

                TotalDrivers =
                    drivers.Count,

                AvailableDrivers =
                    drivers.Count(d =>
                        d.IsAvailable),

                RecentDeliveries =
                    deliveries
                        .OrderByDescending(
                            d => d.CreatedDate)
                        .Take(5)
                        .ToList()
            };

            return View(model);
        }
    }
}