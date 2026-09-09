using Azure;
using Azure.Data.Tables;
using BYTE_ME_CoffeeNChill.Models;
using Microsoft.Extensions.Configuration;

namespace BYTE_ME_CoffeeNChill.Services;

public class MenuTableService
{
    private readonly TableClient _tableClient;

    public MenuTableService(IConfiguration configuration)
    {
        var connectionString =
            configuration["MenuStorageConnection"]
            ?? configuration["AzureWebJobsStorage"]
            ?? throw new InvalidOperationException(
                "Menu storage connection string is not configured.");

        var tableName =
            configuration["MenuTableName"]
            ?? "MenuItems";

        _tableClient =
            new TableClient(
                connectionString,
                tableName);
    }

    public async Task InitializeAsync()
    {
        await _tableClient.CreateIfNotExistsAsync();
    }

    public async Task<MenuItemEntity> CreateAsync(
        CreateMenuItemRequest request)
    {
        await InitializeAsync();

        var entity = new MenuItemEntity
        {
            PartitionKey = request.Category.Trim(),

            RowKey = request.Id.Trim(),

            Name = request.Name.Trim(),

            Description =
                request.Description?.Trim()
                ?? string.Empty,

            Price = request.Price,

            IsAvailable = request.IsAvailable
        };

        await _tableClient.AddEntityAsync(entity);

        return entity;
    }

    public async Task<List<MenuItemEntity>> GetAllAsync()
    {
        await InitializeAsync();

        var results =
            new List<MenuItemEntity>();

        await foreach (
            var entity in
            _tableClient.QueryAsync<MenuItemEntity>())
        {
            results.Add(entity);
        }

        return results;
    }

    public async Task<List<MenuItemEntity>>
        GetByCategoryAsync(string category)
    {
        await InitializeAsync();

        var results =
            new List<MenuItemEntity>();

        string filter =
            TableClient.CreateQueryFilter(
                $"PartitionKey eq {category}");

        await foreach (
            var entity in
            _tableClient.QueryAsync<MenuItemEntity>(
                filter))
        {
            results.Add(entity);
        }

        return results;
    }

    public async Task<MenuItemEntity?> GetAsync(
        string category,
        string id)
    {
        await InitializeAsync();

        try
        {
            var response =
                await _tableClient
                    .GetEntityAsync<MenuItemEntity>(
                        category,
                        id);

            return response.Value;
        }
        catch (RequestFailedException ex)
            when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<MenuItemEntity?> UpdateAsync(
        string category,
        string id,
        UpdateMenuItemRequest request)
    {
        var entity =
            await GetAsync(
                category,
                id);

        if (entity is null)
        {
            return null;
        }

        if (request.Price.HasValue)
        {
            entity.Price =
                request.Price.Value;
        }

        if (request.IsAvailable.HasValue)
        {
            entity.IsAvailable =
                request.IsAvailable.Value;
        }

        await _tableClient.UpdateEntityAsync(
            entity,
            ETag.All,
            TableUpdateMode.Merge);

        return entity;
    }

    public async Task<bool> DeleteAsync(
        string category,
        string id)
    {
        await InitializeAsync();

        try
        {
            await _tableClient.DeleteEntityAsync(
                category,
                id);

            return true;
        }
        catch (RequestFailedException ex)
            when (ex.Status == 404)
        {
            return false;
        }
    }

    public static MenuItemResponse ToResponse(
        MenuItemEntity entity)
    {
        return new MenuItemResponse
        {
            Category =
                entity.PartitionKey,

            Id =
                entity.RowKey,

            Name =
                entity.Name,

            Description =
                entity.Description,

            Price =
                entity.Price,

            IsAvailable =
                entity.IsAvailable,

            LastModified =
                entity.Timestamp
        };
    }
}