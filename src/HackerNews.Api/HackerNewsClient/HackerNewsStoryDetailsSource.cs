using System.Net.Http.Json;
using HackerNews.Api.Contracts;

namespace HackerNews.Api.HackerNewsClient;

public sealed class HackerNewsStoryDetailsSource(HttpClient httpClient) : IStoryDetailsSource
{
    public async Task<StoryResponse?> GetStoryAsync(int id, CancellationToken cancellationToken)
    {
        var item = await httpClient.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json", cancellationToken);

        if (item is null || item.Deleted || item.Dead || item.Type != "story")
        {
            return null;
        }

        return new StoryResponse(
            item.Title ?? string.Empty,
            item.Url ?? string.Empty,
            item.By ?? string.Empty,
            DateTimeOffset.FromUnixTimeSeconds(item.Time),
            item.Score ?? 0,
            item.Descendants ?? 0);
    }
}
