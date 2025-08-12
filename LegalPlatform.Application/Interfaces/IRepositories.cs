using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task UpdateAsync(User user, CancellationToken cancellationToken);
}

public interface ILawyerRepository
{
    Task<LawyerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IEnumerable<LawyerProfile>> GetApprovedAsync(CancellationToken cancellationToken);
    Task<IEnumerable<LawyerProfile>> GetPendingAsync(CancellationToken cancellationToken);
    Task AddAsync(LawyerProfile profile, CancellationToken cancellationToken);
    Task UpdateAsync(LawyerProfile profile, CancellationToken cancellationToken);
}

public interface IDocumentRepository
{
    Task AddAsync(Document document, CancellationToken cancellationToken);
    Task<IEnumerable<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Document?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken);
}

public interface IChatRepository
{
    Task AddAsync(ChatHistory chat, CancellationToken cancellationToken);
    Task<IEnumerable<ChatHistory>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken);
}

public interface IEmailOtpRepository
{
    Task AddAsync(EmailOtp otp, CancellationToken cancellationToken);
    Task<EmailOtp?> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task MarkUsedAsync(EmailOtp otp, CancellationToken cancellationToken);
}