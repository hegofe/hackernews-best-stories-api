namespace HackerNews.Api.HackerNewsClient;

public interface IBestStoriesSource
{
    Task<IReadOnlyList<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken);
}
