using HackerNews.Api.Caching;
using HackerNews.Api.HackerNewsClient;
using Microsoft.Extensions.Options;

namespace HackerNews.Api.BackgroundServices;

public sealed class StoriesRefreshBackgroundService(
    IBestStoriesSource bestStoriesSource,
    IStoryDetailsSource detailsSource,
    IStoriesCache cache,
    IOptionsMonitor<StoriesCacheOptions> options,
    ILogger<StoriesRefreshBackgroundService> logger) : BackgroundService
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(options.CurrentValue.RefreshIntervalSeconds), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await RefreshAsync(stoppingToken);
        }
    }

    private async Task RefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            var allIds = await bestStoriesSource.GetBestStoryIdsAsync(cancellationToken);
            var idsToCache = allIds.Take(options.CurrentValue.MaxCachedStories).ToList();

            var stories = await StoryBatchFetcher.FetchAsync(
                detailsSource,
                idsToCache,
                options.CurrentValue.MaxDegreeOfParallelism,
                cancellationToken);

            cache.Update(new StoriesSnapshot(allIds, stories));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh Hacker News best stories cache");
        }
    }
}
