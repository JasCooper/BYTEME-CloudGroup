using BYTE_ME_CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BYTE_ME_CoffeeNChill.Functions;

public class DocumentFunctions
{
    private readonly StaffBlobService
        _blobService;

    private readonly ILogger<DocumentFunctions>
        _logger;

    public DocumentFunctions(
        StaffBlobService blobService,
        ILogger<DocumentFunctions> logger)
    {
        _blobService =
            blobService;

        _logger =
            logger;
    }

    // =========================================
    // UPLOAD DOCUMENT
    // POST /api/documents/upload
    // =========================================

    [Function("UploadStaffDocument")]
    public async Task<IActionResult>
        UploadStaffDocument(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route =
                "documents/upload")]
        HttpRequest req)
    {
        try
        {
            if (!req.HasFormContentType)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Use multipart/form-data and attach a file."
                    });
            }

            var form =
                await req.ReadFormAsync();

            var file =
                form.Files.GetFile("file")
                ??
                form.Files.FirstOrDefault();

            if (file is null
                ||
                file.Length == 0)
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "A non-empty file is required."
                    });
            }

            var safeFileName =
                Path.GetFileName(
                    file.FileName);

            if (string.IsNullOrWhiteSpace(
                safeFileName))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "Invalid file name."
                    });
            }

            await using var stream =
                file.OpenReadStream();

            await _blobService.UploadAsync(
                safeFileName,
                stream,
                file.ContentType);

            return new ObjectResult(
                new
                {
                    message =
                        "Staff document uploaded successfully.",

                    fileName =
                        safeFileName,

                    size =
                        file.Length
                })
            {
                StatusCode =
                    StatusCodes
                        .Status201Created
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to upload staff document.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to upload staff document."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // LIST DOCUMENTS
    // GET /api/documents
    // =========================================

    [Function("ListStaffDocuments")]
    public async Task<IActionResult>
        ListStaffDocuments(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents")]
        HttpRequest req)
    {
        try
        {
            var files =
                await _blobService
                    .ListAsync();

            return new OkObjectResult(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to list documents.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to list staff documents."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }

    // =========================================
    // DOWNLOAD DOCUMENT
    // GET /api/documents/download/{fileName}
    // =========================================

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult>
        DownloadStaffDocument(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route =
                "documents/download/{fileName}")]
        HttpRequest req,
        string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(
                fileName))
            {
                return new BadRequestObjectResult(
                    new
                    {
                        error =
                            "File name is required."
                    });
            }

            var result =
                await _blobService
                    .DownloadAsync(
                        fileName);

            if (result is null)
            {
                return new NotFoundObjectResult(
                    new
                    {
                        error =
                            "Document not found."
                    });
            }

            return new FileStreamResult(
                result.Value.Stream,
                result.Value.ContentType)
            {
                FileDownloadName =
                    Path.GetFileName(
                        fileName)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to download document.");

            return new ObjectResult(
                new
                {
                    error =
                        "Unable to download staff document."
                })
            {
                StatusCode =
                    StatusCodes
                        .Status500InternalServerError
            };
        }
    }
}