using HackerNews.Api.Contracts;

namespace HackerNews.Api.HackerNewsClient;

public interface IStoryDetailsSource
{
    Task<StoryResponse?> GetStoryAsync(int id, CancellationToken cancellationToken);
}
