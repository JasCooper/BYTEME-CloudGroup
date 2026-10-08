using ByteMeLogistics.Web.Models;
using ByteMeLogistics.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ByteMeLogistics.Web.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly ApiService _apiService;

        public DeliveriesController(
            ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(
            string? search,
            DeliveryStatus? status)
        {
            var deliveries =
                await _apiService.GetDeliveriesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                deliveries = deliveries.Where(d =>
                    d.TrackingNumber.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ||

                    d.PackageDescription.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase) ||

                    (d.CustomerName?.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)
                        ?? false))
                    .ToList();
            }

            if (status.HasValue)
            {
                deliveries = deliveries
                    .Where(d => d.Status == status.Value)
                    .ToList();
            }

            return View(deliveries);
        }

        public async Task<IActionResult> Details(int id)
        {
            var delivery =
                await _apiService.GetDeliveryAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            return View(
                new DeliveryViewModel
                {
                    ExpectedDeliveryDate =
                        DateTime.Today.AddDays(2)
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(
                    model.CustomerId,
                    model.DriverId);

                return View(model);
            }

            bool success =
                await _apiService
                    .CreateDeliveryAsync(model);

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create delivery.");

                await LoadDropdowns(
                    model.CustomerId,
                    model.DriverId);

                return View(model);
            }

            TempData["Success"] =
                "Delivery created successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var delivery =
                await _apiService.GetDeliveryAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            await LoadDropdowns(
                delivery.CustomerId,
                delivery.DriverId);

            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            DeliveryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(
                    model.CustomerId,
                    model.DriverId);

                return View(model);
            }

            await _apiService.UpdateDeliveryAsync(model);

            TempData["Success"] =
                "Delivery updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            DeliveryStatus status)
        {
            await _apiService
                .UpdateDeliveryStatusAsync(id, status);

            TempData["Success"] =
                "Delivery status updated.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var delivery =
                await _apiService.GetDeliveryAsync(id);

            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            await _apiService.DeleteDeliveryAsync(id);

            TempData["Success"] =
                "Delivery deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns(
            int? customerId = null,
            int? driverId = null)
        {
            var customers =
                await _apiService.GetCustomersAsync();

            var drivers =
                await _apiService.GetDriversAsync();

            ViewBag.Customers = new SelectList(
                customers,
                "CustomerId",
                "Name",
                customerId);

            ViewBag.Drivers = new SelectList(
                drivers,
                "DriverId",
                "Name",
                driverId);
        }
    }
}