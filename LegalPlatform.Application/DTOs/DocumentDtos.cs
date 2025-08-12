namespace LegalPlatform.Application.DTOs;

public class DocumentMetadataDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class ChatRequest
{
    public string Query { get; set; } = string.Empty;
}

public class NewsInterestRequest
{
    public string Interest { get; set; } = string.Empty;
}