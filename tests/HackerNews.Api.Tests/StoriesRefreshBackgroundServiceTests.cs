using HackerNews.Api.BackgroundServices;
using HackerNews.Api.Caching;
using HackerNews.Api.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace HackerNews.Api.Tests;

public class StoriesRefreshBackgroundServiceTests
{
    private static StoriesRefreshBackgroundService CreateService(
        FakeBestStoriesSource bestStoriesSource,
        FakeStoryDetailsSource detailsSource,
        StoriesCache cache,
        int maxCachedStories)
    {
        var options = new StoriesCacheOptions
        {
            RefreshIntervalSeconds = 3600,
            MaxCachedStories = maxCachedStories,
            MaxDegreeOfParallelism = 4
        };

        return new StoriesRefreshBackgroundService(
            bestStoriesSource,
            detailsSource,
            cache,
            new TestOptionsMonitor<StoriesCacheOptions>(options),
            NullLogger<StoriesRefreshBackgroundService>.Instance);
    }

    [Fact]
    public async Task StartAsync_PopulatesCacheBeforeReturning()
    {
        int[] ids = [5, 4, 3, 2, 1];
        var cache = new StoriesCache();
        var service = CreateService(new FakeBestStoriesSource(ids), new FakeStoryDetailsSource(ids), cache, maxCachedStories: 10);

        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal(ids, cache.Current.AllIds);
        Assert.Equal(ids, cache.Current.CachedStories.Select(s => s.Score));
    }

    [Fact]
    public async Task StartAsync_LimitsCachedStoriesToConfiguredMaximumButKeepsAllIds()
    {
        int[] ids = [5, 4, 3, 2, 1];
        var cache = new StoriesCache();
        var service = CreateService(new FakeBestStoriesSource(ids), new FakeStoryDetailsSource(ids), cache, maxCachedStories: 3);

        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Equal(ids, cache.Current.AllIds);
        Assert.Equal([5, 4, 3], cache.Current.CachedStories.Select(s => s.Score));
    }

    [Fact]
    public async Task StartAsync_WhenFetchFails_LeavesExistingCacheUntouched()
    {
        int[] ids = [5, 4, 3];
        var cache = new StoriesCache();
        var previousSnapshot = new StoriesSnapshot(ids, ids.Select(StoryFactory.Create).ToList());
        cache.Update(previousSnapshot);

        var bestStoriesSource = new FakeBestStoriesSource(ids) { ShouldFail = true };
        var service = CreateService(bestStoriesSource, new FakeStoryDetailsSource(ids), cache, maxCachedStories: 10);

        await service.StartAsync(CancellationToken.None);
        await service.StopAsync(CancellationToken.None);

        Assert.Same(previousSnapshot, cache.Current);
    }
}
