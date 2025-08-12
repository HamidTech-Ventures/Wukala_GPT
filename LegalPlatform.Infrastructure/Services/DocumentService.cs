using LegalPlatform.Application.DTOs;
using LegalPlatform.Application.Interfaces;
using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Infrastructure.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documents;
    private readonly IEncryptionService _encryption;

    public DocumentService(IDocumentRepository documents, IEncryptionService encryption)
    {
        _documents = documents;
        _encryption = encryption;
    }

    public async Task UploadAsync(Guid userId, string originalFileName, string contentType, byte[] content, CancellationToken cancellationToken)
    {
        var (cipher, iv) = _encryption.EncryptAes(content);
        var doc = new Document
        {
            UserId = userId,
            OriginalFileName = originalFileName,
            ContentType = contentType,
            SizeBytes = content.LongLength,
            EncryptedContent = cipher,
            EncryptionIvBase64 = Convert.ToBase64String(iv)
        };
        await _documents.AddAsync(doc, cancellationToken);
    }

    public async Task<IEnumerable<DocumentMetadataDto>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var docs = await _documents.GetByUserAsync(userId, cancellationToken);
        return docs.Select(d => new DocumentMetadataDto
        {
            Id = d.Id,
            OriginalFileName = d.OriginalFileName,
            ContentType = d.ContentType,
            SizeBytes = d.SizeBytes,
            CreatedAtUtc = d.CreatedAtUtc
        });
    }

    public async Task<(string fileName, string contentType, byte[] data)> DownloadAsync(Guid userId, Guid documentId, CancellationToken cancellationToken)
    {
        var doc = await _documents.GetByIdAsync(documentId, cancellationToken) ?? throw new KeyNotFoundException("Document not found");
        if (doc.UserId != userId) throw new UnauthorizedAccessException("Not your document");
        var iv = Convert.FromBase64String(doc.EncryptionIvBase64);
        var data = _encryption.DecryptAes(doc.EncryptedContent, iv);
        return (doc.OriginalFileName, doc.ContentType, data);
    }
}