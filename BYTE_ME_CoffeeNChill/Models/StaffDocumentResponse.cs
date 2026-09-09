namespace BYTE_ME_CoffeeNChill.Models;

public class StaffDocumentResponse
{
    public string FileName { get; set; } = string.Empty;

    public long Size { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}