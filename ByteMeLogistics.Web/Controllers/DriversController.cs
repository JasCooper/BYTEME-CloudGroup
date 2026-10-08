using ByteMeLogistics.Web.Models;
using ByteMeLogistics.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ByteMeLogistics.Web.Controllers
{
    public class DriversController : Controller
    {
        private readonly ApiService _apiService;

        public DriversController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            return View(
                await _apiService.GetDriversAsync());
        }

        public IActionResult Create()
        {
            return View(
                new DriverViewModel
                {
                    IsAvailable = true
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _apiService.CreateDriverAsync(model);

            TempData["Success"] =
                "Driver added successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var driver =
                await _apiService.GetDriverAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _apiService.UpdateDriverAsync(model);

            TempData["Success"] =
                "Driver updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var driver =
                await _apiService.GetDriverAsync(id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeleteConfirmed(int id)
        {
            await _apiService.DeleteDriverAsync(id);

            TempData["Success"] =
                "Driver removed successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}