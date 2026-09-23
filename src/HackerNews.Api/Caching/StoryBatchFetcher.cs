using HackerNews.Api.Contracts;
using HackerNews.Api.HackerNewsClient;

namespace HackerNews.Api.Caching;

public static class StoryBatchFetcher
{
    public static async Task<IReadOnlyList<StoryResponse>> FetchAsync(
        IStoryDetailsSource detailsSource,
        IReadOnlyList<int> ids,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken)
    {
        var results = new StoryResponse?[ids.Count];

        await Parallel.ForEachAsync(
            Enumerable.Range(0, ids.Count),
            new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken
            },
            async (index, token) =>
            {
                results[index] = await detailsSource.GetStoryAsync(ids[index], token);
            });

        return results.Where(story => story is not null).Select(story => story!).ToList();
    }
}
