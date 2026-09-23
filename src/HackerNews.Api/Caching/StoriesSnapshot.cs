using HackerNews.Api.Contracts;

namespace HackerNews.Api.Caching;

public sealed record StoriesSnapshot(IReadOnlyList<int> AllIds, IReadOnlyList<StoryResponse> CachedStories)
{
    public static StoriesSnapshot Empty { get; } = new([], []);
}
