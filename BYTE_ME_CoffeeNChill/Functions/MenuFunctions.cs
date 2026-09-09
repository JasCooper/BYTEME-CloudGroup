using System.Text.Json;
using Azure;
using BYTE_ME_CoffeeNChill.Models;
using BYTE_ME_CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BYTE_ME_CoffeeNChill.Functions;

public class MenuFunctions
{
    private readonly MenuTableService _menuService;

    private readonly ILogger<MenuFunctions> _logger;

    private static readonly JsonSerializerOptions
        JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

    public MenuFunctions(
        MenuTableService menuService,
        ILogger<MenuFunctions> logger)
    {
        _menuService = menuService;

        _logger = logger;
    }

    // =========================================
    // CREATE MENU ITEM
    // POST /api/menu
    // =========================================

    [Function("CreateMenuItem")]
    public async Task<IActionResult> CreateMenuItem(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "menu")]
        HttpRequest req)
    {
        try
        {
            var request =
                await JsonSerializer
                    .DeserializeAsync<CreateMenuItemRequest>(
                        req.Body,
                        JsonOptions);

            if (request is null)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "A JSON body is required."
                    });
            }

            if (string.IsNullOrWhiteSpace(
                request.Category))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Category is required."
                    });
            }

            if (string.IsNullOrWhiteSpace(
                request.Id))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "ID/SKU is required."
                    });
            }

            if (string.IsNullOrWhiteSpace(
                request.Name))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Name is required."
                    });
            }

            if (request.Price < 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Price cannot be negative."
                    });
            }

            var entity =
                await _menuService
                    .CreateAsync(request);

            var response =
                MenuTableService
                    .ToResponse(entity);

            return new ObjectResult(response)
            {
                StatusCode =
                    StatusCodes
                        .Status201Created
            };
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(
                new
                {
                    error =
                        "The JSON body is invalid."
                });
        }
        catch (RequestFailedException ex)
            when (ex.Status == 409)
        {
            return new ConflictObjectResult(
                new
                {
                    error =
                        "A menu item with the same category and ID already exists."
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create menu item.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to create menu item."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // GET ALL MENU ITEMS
    // GET /api/menu
    // =========================================

    [Function("GetAllMenuItems")]
    public async Task<IActionResult>
        GetAllMenuItems(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "menu")]
        HttpRequest req)
    {
        try
        {
            var items =
                await _menuService
                    .GetAllAsync();

            var result =
                items.Select(
                    MenuTableService
                        .ToResponse);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to retrieve menu items.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to retrieve menu items."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // GET ITEMS BY CATEGORY
    // GET /api/menu/category/{category}
    // =========================================

    [Function("GetMenuItemsByCategory")]
    public async Task<IActionResult>
        GetMenuItemsByCategory(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route =
                "menu/category/{category}")]
        HttpRequest req,
        string category)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(
                category))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Category is required."
                    });
            }

            var items =
                await _menuService
                    .GetByCategoryAsync(
                        category);

            var result =
                items.Select(
                    MenuTableService
                        .ToResponse);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to retrieve menu category.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to retrieve menu items."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // UPDATE MENU ITEM
    // PUT /api/menu/{category}/{id}
    // =========================================

    [Function("UpdateMenuItem")]
    public async Task<IActionResult>
        UpdateMenuItem(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "put",
            Route =
                "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        try
        {
            var request =
                await JsonSerializer
                    .DeserializeAsync
                    <UpdateMenuItemRequest>(
                        req.Body,
                        JsonOptions);

            if (request is null)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "A JSON body is required."
                    });
            }

            if (!request.Price.HasValue
                &&
                !request.IsAvailable.HasValue)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Supply price and/or isAvailable."
                    });
            }

            if (request.Price.HasValue
                &&
                request.Price.Value < 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Price cannot be negative."
                    });
            }

            var updated =
                await _menuService
                    .UpdateAsync(
                        category,
                        id,
                        request);

            if (updated is null)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        error =
                            "Menu item not found."
                    });
            }

            return new OkObjectResult(
                MenuTableService
                    .ToResponse(updated));
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(
                new
                {
                    error =
                        "Invalid JSON."
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to update menu item.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to update menu item."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // DELETE MENU ITEM
    // DELETE /api/menu/{category}/{id}
    // =========================================

    [Function("DeleteMenuItem")]
    public async Task<IActionResult>
        DeleteMenuItem(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "delete",
            Route =
                "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        try
        {
            var deleted =
                await _menuService
                    .DeleteAsync(
                        category,
                        id);

            if (!deleted)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        error =
                            "Menu item not found."
                    });
            }

            return new OkObjectResult(
                new
                {
                    message =
                        "Menu item deleted successfully.",

                    category,

                    id
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to delete menu item.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to delete menu item."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }
}