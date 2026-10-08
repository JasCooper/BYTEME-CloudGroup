using ByteMeLogistics.Web.Models;
using System.Net.Http.Json;

namespace ByteMeLogistics.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // CUSTOMERS

        public async Task<List<CustomerViewModel>>
            GetCustomersAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<CustomerViewModel>>(
                    "api/customers")
                ?? new List<CustomerViewModel>();
        }

        public async Task<CustomerViewModel?>
            GetCustomerAsync(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<CustomerViewModel>(
                    $"api/customers/{id}");
        }

        public async Task<bool> CreateCustomerAsync(
            CustomerViewModel customer)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/customers",
                    customer);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCustomerAsync(
            CustomerViewModel customer)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/customers/{customer.CustomerId}",
                    customer);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/customers/{id}");

            return response.IsSuccessStatusCode;
        }

        // DRIVERS

        public async Task<List<DriverViewModel>>
            GetDriversAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<DriverViewModel>>(
                    "api/drivers")
                ?? new List<DriverViewModel>();
        }

        public async Task<DriverViewModel?>
            GetDriverAsync(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<DriverViewModel>(
                    $"api/drivers/{id}");
        }

        public async Task<bool> CreateDriverAsync(
            DriverViewModel driver)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/drivers",
                    driver);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDriverAsync(
            DriverViewModel driver)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/drivers/{driver.DriverId}",
                    driver);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDriverAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/drivers/{id}");

            return response.IsSuccessStatusCode;
        }

        // DELIVERIES

        public async Task<List<DeliveryViewModel>>
            GetDeliveriesAsync()
        {
            return await _httpClient
                .GetFromJsonAsync<List<DeliveryViewModel>>(
                    "api/deliveries")
                ?? new List<DeliveryViewModel>();
        }

        public async Task<DeliveryViewModel?>
            GetDeliveryAsync(int id)
        {
            return await _httpClient
                .GetFromJsonAsync<DeliveryViewModel>(
                    $"api/deliveries/{id}");
        }

        public async Task<bool> CreateDeliveryAsync(
            DeliveryViewModel delivery)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "api/deliveries",
                    delivery);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDeliveryAsync(
            DeliveryViewModel delivery)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/deliveries/{delivery.DeliveryId}",
                    delivery);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDeliveryAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"api/deliveries/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateDeliveryStatusAsync(
            int id,
            DeliveryStatus status)
        {
            var response =
                await _httpClient.PatchAsync(
                    $"api/deliveries/{id}/status?status={status}",
                    null);

            return response.IsSuccessStatusCode;
        }
    }
}