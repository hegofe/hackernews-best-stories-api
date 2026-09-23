using HackerNews.Api.Caching;
using HackerNews.Api.Contracts;
using HackerNews.Api.HackerNewsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HackerNews.Api.Controllers;

[ApiController]
[Route("api/stories")]
public sealed class StoriesController(
    IStoriesCache cache,
    IStoryDetailsSource detailsSource,
    IOptionsMonitor<StoriesCacheOptions> options) : ControllerBase
{
    [HttpGet("best")]
    public async Task<ActionResult<IReadOnlyList<StoryResponse>>> GetBest([FromQuery] int n, CancellationToken cancellationToken)
    {
        if (n <= 0)
        {
            return Ok(Array.Empty<StoryResponse>());
        }

        var snapshot = cache.Current;
        var cachedCount = snapshot.CachedStories.Count;

        if (n <= cachedCount)
        {
            return Ok(snapshot.CachedStories.Take(n).ToList());
        }

        var overflowIds = snapshot.AllIds.Skip(cachedCount).Take(n - cachedCount).ToList();

        if (overflowIds.Count == 0)
        {
            return Ok(snapshot.CachedStories);
        }

        var overflowStories = await StoryBatchFetcher.FetchAsync(
            detailsSource,
            overflowIds,
            options.CurrentValue.MaxDegreeOfParallelism,
            cancellationToken);

        return Ok(snapshot.CachedStories.Concat(overflowStories).ToList());
    }
}
