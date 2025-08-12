using LegalPlatform.Application.DTOs;
using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user, DateTime? expires = null);
}

public interface IEmailService
{
    Task SendOtpAsync(string toEmail, string code, CancellationToken cancellationToken);
    Task SendLawyerApprovalAsync(string toEmail, CancellationToken cancellationToken);
    Task SendLawyerRejectionAsync(string toEmail, string reason, CancellationToken cancellationToken);
}

public interface IEncryptionService
{
    (byte[] cipherBytes, byte[] iv) EncryptAes(byte[] plainBytes);
    byte[] DecryptAes(byte[] cipherBytes, byte[] iv);
    string EncryptString(string plainText);
    string DecryptString(string cipherBase64);
}

public interface IAuthService
{
    Task<AuthResponse> SignUpLocalPersonAsync(SignUpLocalPersonRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> SignUpLawyerAsync(SignUpLawyerRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> VerifyOtpAsync(OtpVerifyRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

public interface IDocumentService
{
    Task UploadAsync(Guid userId, string originalFileName, string contentType, byte[] content, CancellationToken cancellationToken);
    Task<IEnumerable<DocumentMetadataDto>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<(string fileName, string contentType, byte[] data)> DownloadAsync(Guid userId, Guid documentId, CancellationToken cancellationToken);
}

public interface IChatService
{
    Task<string> AskAiAsync(Guid userId, string query, CancellationToken cancellationToken);
    Task<Guid> SendSecureMessageAsync(Guid conversationId, Guid senderUserId, string message, CancellationToken cancellationToken);
    Task<Guid> SendSecureFileAsync(Guid conversationId, Guid senderUserId, string fileName, string contentType, byte[] bytes, CancellationToken cancellationToken);
    Task<IEnumerable<(DateTime createdAt, string message)>> GetSecureConversationAsync(Guid conversationId, Guid requesterUserId, CancellationToken cancellationToken);
}

public interface ILawyerService
{
    Task<IEnumerable<object>> GetApprovedLawyersAsync(CancellationToken cancellationToken);
    Task UpdateLawyerProfileAsync(Guid userId, SignUpLawyerRequest request, CancellationToken cancellationToken);
}

public interface IAdminService
{
    Task<IEnumerable<object>> GetPendingLawyersAsync(CancellationToken cancellationToken);
    Task VerifyLawyerAsync(Guid lawyerUserId, CancellationToken cancellationToken);
    Task RejectLawyerAsync(Guid lawyerUserId, string reason, CancellationToken cancellationToken);
}

public interface INewsService
{
    Task SetUserInterestAsync(Guid userId, string interest, CancellationToken cancellationToken);
    Task<IEnumerable<object>> GetNewsAsync(Guid userId, CancellationToken cancellationToken);
}