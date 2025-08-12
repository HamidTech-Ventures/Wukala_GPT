using System.Net.Http.Json;
using LegalPlatform.Application.Interfaces;
using LegalPlatform.Domain.Entities;

namespace LegalPlatform.Infrastructure.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chats;
    private readonly IEncryptionService _encryption;
    private readonly HttpClient _httpClient;

    public ChatService(IChatRepository chats, IEncryptionService encryption, IHttpClientFactory httpClientFactory)
    {
        _chats = chats;
        _encryption = encryption;
        _httpClient = httpClientFactory.CreateClient("ai");
    }

    public async Task<string> AskAiAsync(Guid userId, string query, CancellationToken cancellationToken)
    {
        var resp = await _httpClient.PostAsJsonAsync("/v1/ask", new { query }, cancellationToken);
        var content = await resp.Content.ReadAsStringAsync(cancellationToken);
        await _chats.AddAsync(new ChatHistory
        {
            UserId = userId,
            Query = query,
            Response = content,
            IsSecure = false
        }, cancellationToken);
        return content;
    }

    public async Task<Guid> SendSecureMessageAsync(Guid conversationId, Guid senderUserId, string message, CancellationToken cancellationToken)
    {
        var cipher = _encryption.EncryptString(message);
        var chat = new ChatHistory
        {
            UserId = senderUserId,
            ConversationId = conversationId,
            IsSecure = true,
            Query = "MSG",
            Response = cipher
        };
        await _chats.AddAsync(chat, cancellationToken);
        return conversationId == Guid.Empty ? chat.Id : conversationId;
    }

    public async Task<Guid> SendSecureFileAsync(Guid conversationId, Guid senderUserId, string fileName, string contentType, byte[] bytes, CancellationToken cancellationToken)
    {
        var cipher = _encryption.EncryptString($"FILE:{fileName}:{contentType}:{Convert.ToBase64String(bytes)}");
        var chat = new ChatHistory
        {
            UserId = senderUserId,
            ConversationId = conversationId,
            IsSecure = true,
            Query = "FILE",
            Response = cipher
        };
        await _chats.AddAsync(chat, cancellationToken);
        return conversationId == Guid.Empty ? chat.Id : conversationId;
    }

    public async Task<IEnumerable<(DateTime createdAt, string message)>> GetSecureConversationAsync(Guid conversationId, Guid requesterUserId, CancellationToken cancellationToken)
    {
        var history = await _chats.GetByConversationAsync(conversationId, cancellationToken);
        return history.Where(h => h.IsSecure).Select(h => (h.CreatedAtUtc, _encryption.DecryptString(h.Response)));
    }
}