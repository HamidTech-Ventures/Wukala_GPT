using System.Net.Http.Json;
using LegalPlatform.Application.Interfaces;

namespace LegalPlatform.Infrastructure.Services;

public class NewsService : INewsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Dictionary<Guid, string> _userInterests = new();

    public NewsService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public Task SetUserInterestAsync(Guid userId, string interest, CancellationToken cancellationToken)
    {
        _userInterests[userId] = interest;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<object>> GetNewsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var interest = _userInterests.TryGetValue(userId, out var i) ? i : "general";
        var newsClient = _httpClientFactory.CreateClient("news");
        var newsResp = await newsClient.GetAsync($"/v1/news?interest={Uri.EscapeDataString(interest)}", cancellationToken);
        var newsJson = await newsResp.Content.ReadAsStringAsync(cancellationToken);

        var mlClient = _httpClientFactory.CreateClient("ml");
        var mlResp = await mlClient.PostAsJsonAsync("/v1/rank", new { interest, items = newsJson }, cancellationToken);
        var ranked = await mlResp.Content.ReadAsStringAsync(cancellationToken);

        return new[] { new { interest, ranked } };
    }
}