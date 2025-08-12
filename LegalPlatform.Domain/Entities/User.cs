namespace LegalPlatform.Domain.Entities;

public enum UserRole
{
    LocalPerson = 1,
    Lawyer = 2,
    Admin = 3
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? City { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = false; // activated after email OTP
    public bool EmailVerified { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public LawyerProfile? LawyerProfile { get; set; }
    public ICollection<ChatHistory> ChatHistories { get; set; } = new List<ChatHistory>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}