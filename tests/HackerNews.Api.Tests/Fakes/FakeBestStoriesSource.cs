using HackerNews.Api.HackerNewsClient;

namespace HackerNews.Api.Tests.Fakes;

public sealed class FakeBestStoriesSource(IReadOnlyList<int> ids) : IBestStoriesSource
{
    public bool ShouldFail { get; set; }

    public Task<IReadOnlyList<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
    {
        if (ShouldFail)
        {
            throw new HttpRequestException("Simulated failure");
        }

        return Task.FromResult(ids);
    }
}
