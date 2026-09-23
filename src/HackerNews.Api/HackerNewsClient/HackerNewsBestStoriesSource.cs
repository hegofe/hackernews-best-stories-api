using System.Net.Http.Json;

namespace HackerNews.Api.HackerNewsClient;

public sealed class HackerNewsBestStoriesSource(HttpClient httpClient) : IBestStoriesSource
{
    public async Task<IReadOnlyList<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        var ids = await httpClient.GetFromJsonAsync<int[]>("beststories.json", cancellationToken);
        return ids ?? [];
    }
}
