using System.Collections.Concurrent;
using HackerNews.Api.Contracts;
using HackerNews.Api.HackerNewsClient;

namespace HackerNews.Api.Tests.Fakes;

public sealed class FakeStoryDetailsSource(IEnumerable<int> availableIds) : IStoryDetailsSource
{
    private readonly HashSet<int> _availableIds = availableIds.ToHashSet();

    public ConcurrentBag<int> RequestedIds { get; } = [];

    public bool ShouldFail { get; set; }

    public Task<StoryResponse?> GetStoryAsync(int id, CancellationToken cancellationToken)
    {
        if (ShouldFail)
        {
            throw new HttpRequestException("Simulated failure");
        }

        RequestedIds.Add(id);

        StoryResponse? story = _availableIds.Contains(id) ? StoryFactory.Create(id) : null;
        return Task.FromResult(story);
    }
}
