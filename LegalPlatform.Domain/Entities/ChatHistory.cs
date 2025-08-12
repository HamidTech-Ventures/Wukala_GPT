namespace LegalPlatform.Domain.Entities;

public class ChatHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? ConversationId { get; set; } // for secure chats
    public bool IsSecure { get; set; } = false;

    public string Query { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty; // encrypted when IsSecure
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}