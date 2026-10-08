using ByteMeLogistics.Web.Models;
using ByteMeLogistics.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ByteMeLogistics.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApiService _apiService;

        public CustomersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(
            string? search)
        {
            var customers =
                await _apiService.GetCustomersAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                customers = customers
                    .Where(c =>
                        c.Name.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        c.Email.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        c.PhoneNumber.Contains(search))
                    .ToList();
            }

            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool success =
                await _apiService
                    .CreateCustomerAsync(model);

            if (!success)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create customer.");

                return View(model);
            }

            TempData["Success"] =
                "Customer added successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer =
                await _apiService.GetCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _apiService
                .UpdateCustomerAsync(model);

            TempData["Success"] =
                "Customer updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var customer =
                await _apiService.GetCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            bool success =
                await _apiService
                    .DeleteCustomerAsync(id);

            if (!success)
            {
                TempData["Error"] =
                    "Customer cannot be deleted while deliveries are linked to them.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}