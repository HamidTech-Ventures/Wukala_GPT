using LegalPlatform.Application.Interfaces;
using LegalPlatform.Domain.Entities;
using LegalPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LegalPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await _db.Users.Include(u => u.LawyerProfile).FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _db.Users.Include(u => u.LawyerProfile).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

public class LawyerRepository : ILawyerRepository
{
    private readonly AppDbContext _db;
    public LawyerRepository(AppDbContext db) => _db = db;

    public async Task<LawyerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => await _db.LawyerProfiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task<IEnumerable<LawyerProfile>> GetApprovedAsync(CancellationToken cancellationToken)
        => await _db.LawyerProfiles.AsNoTracking().Where(x => x.IsVerified && x.Status == "Approved").ToListAsync(cancellationToken);

    public async Task<IEnumerable<LawyerProfile>> GetPendingAsync(CancellationToken cancellationToken)
        => await _db.LawyerProfiles.AsNoTracking().Where(x => !x.IsVerified && x.Status == "Pending").ToListAsync(cancellationToken);

    public async Task AddAsync(LawyerProfile profile, CancellationToken cancellationToken)
    {
        await _db.LawyerProfiles.AddAsync(profile, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(LawyerProfile profile, CancellationToken cancellationToken)
    {
        _db.LawyerProfiles.Update(profile);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _db;
    public DocumentRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(Document document, CancellationToken cancellationToken)
    {
        await _db.Documents.AddAsync(document, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Document>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
        => await _db.Documents.AsNoTracking().Where(d => d.UserId == userId).OrderByDescending(d => d.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task<Document?> GetByIdAsync(Guid documentId, CancellationToken cancellationToken)
        => await _db.Documents.FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);
}

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _db;
    public ChatRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(ChatHistory chat, CancellationToken cancellationToken)
    {
        await _db.ChatHistories.AddAsync(chat, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatHistory>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken)
        => await _db.ChatHistories.AsNoTracking().Where(c => c.ConversationId == conversationId).OrderBy(c => c.CreatedAtUtc).ToListAsync(cancellationToken);
}

public class EmailOtpRepository : IEmailOtpRepository
{
    private readonly AppDbContext _db;
    public EmailOtpRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(EmailOtp otp, CancellationToken cancellationToken)
    {
        await _db.EmailOtps.AddAsync(otp, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmailOtp?> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken)
        => await _db.EmailOtps.FirstOrDefaultAsync(o => o.UserId == userId && !o.Used && o.ExpiresAtUtc > DateTime.UtcNow, cancellationToken);

    public async Task MarkUsedAsync(EmailOtp otp, CancellationToken cancellationToken)
    {
        otp.Used = true;
        _db.EmailOtps.Update(otp);
        await _db.SaveChangesAsync(cancellationToken);
    }
}