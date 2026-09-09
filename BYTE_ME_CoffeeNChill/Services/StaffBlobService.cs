using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BYTE_ME_CoffeeNChill.Models;
using Microsoft.Extensions.Configuration;

namespace BYTE_ME_CoffeeNChill.Services;

public class StaffBlobService
{
    private readonly BlobContainerClient
        _containerClient;

    public StaffBlobService(
        IConfiguration configuration)
    {
        var connectionString =
            configuration["BlobStorageConnection"]
            ?? configuration["AzureWebJobsStorage"]
            ?? throw new InvalidOperationException(
                "Blob storage connection string is not configured.");

        var containerName =
            configuration["StaffDocsContainerName"]
            ?? "staff-docs";

        _containerClient =
            new BlobContainerClient(
                connectionString,
                containerName);
    }

    public async Task InitializeAsync()
    {
        await _containerClient
            .CreateIfNotExistsAsync();
    }

    public async Task UploadAsync(
        string fileName,
        Stream source,
        string? contentType)
    {
        await InitializeAsync();

        var safeFileName =
            Path.GetFileName(fileName);

        var blobClient =
            _containerClient.GetBlobClient(
                safeFileName);

        var options =
            new BlobUploadOptions
            {
                HttpHeaders =
                    new BlobHttpHeaders
                    {
                        ContentType =
                            string.IsNullOrWhiteSpace(
                                contentType)
                                ? GetContentType(
                                    safeFileName)
                                : contentType
                    }
            };

        await blobClient.UploadAsync(
            source,
            options);
    }

    public async Task<List<StaffDocumentResponse>>
        ListAsync()
    {
        await InitializeAsync();

        var results =
            new List<StaffDocumentResponse>();

        await foreach (
            BlobItem blob in
            _containerClient.GetBlobsAsync())
        {
            results.Add(
                new StaffDocumentResponse
                {
                    FileName =
                        blob.Name,

                    Size =
                        blob.Properties
                            .ContentLength
                        ?? 0,

                    LastModified =
                        blob.Properties
                            .LastModified
                });
        }

        return results;
    }

    public async Task<
        (Stream Stream, string ContentType)?>
        DownloadAsync(string fileName)
    {
        await InitializeAsync();

        var safeFileName =
            Path.GetFileName(fileName);

        var blobClient =
            _containerClient.GetBlobClient(
                safeFileName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var response =
            await blobClient
                .DownloadStreamingAsync();

        var contentType =
            response.Value
                .Details
                .ContentType;

        if (string.IsNullOrWhiteSpace(
            contentType))
        {
            contentType =
                GetContentType(
                    safeFileName);
        }

        return (
            response.Value.Content,
            contentType);
    }

    private static string GetContentType(
        string fileName)
    {
        return Path
            .GetExtension(fileName)
            .ToLowerInvariant()
            switch
        {
            ".pdf" =>
                "application/pdf",

            ".txt" =>
                "text/plain",

            ".doc" =>
                "application/msword",

            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            ".xlsx" =>
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

            ".png" =>
                "image/png",

            ".jpg" or ".jpeg" =>
                "image/jpeg",

            _ =>
                "application/octet-stream"
        };
    }
}