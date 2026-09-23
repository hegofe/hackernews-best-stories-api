using HackerNews.Api.Caching;
using HackerNews.Api.Contracts;
using HackerNews.Api.Controllers;
using HackerNews.Api.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Tests;

public class StoriesControllerTests
{
    private static StoriesController CreateController(
        int[] allIds,
        int cachedCount,
        FakeStoryDetailsSource detailsSource)
    {
        var cache = new StoriesCache();
        var cachedStories = allIds.Take(cachedCount).Select(StoryFactory.Create).ToList();
        cache.Update(new StoriesSnapshot(allIds, cachedStories));

        return new StoriesController(
            cache,
            detailsSource,
            new TestOptionsMonitor<StoriesCacheOptions>(new StoriesCacheOptions()));
    }

    private static IReadOnlyList<StoryResponse> GetStories(ActionResult<IReadOnlyList<StoryResponse>> result)
    {
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        return Assert.IsAssignableFrom<IReadOnlyList<StoryResponse>>(ok.Value);
    }

    [Fact]
    public async Task GetBest_WhenRequestedCountIsWithinCache_ReturnsFirstNCachedStoriesInOrder()
    {
        int[] ids = [10, 9, 8, 7, 6];
        var detailsSource = new FakeStoryDetailsSource(ids);
        var controller = CreateController(ids, cachedCount: 5, detailsSource);

        var result = await controller.GetBest(3, CancellationToken.None);

        var stories = GetStories(result);
        Assert.Equal([10, 9, 8], stories.Select(s => s.Score));
        Assert.Empty(detailsSource.RequestedIds);
    }

    [Fact]
    public async Task GetBest_WhenRequestedCountExceedsCache_FetchesMissingStoriesAndMergesInOrder()
    {
        int[] ids = [10, 9, 8, 7, 6];
        var detailsSource = new FakeStoryDetailsSource(ids);
        var controller = CreateController(ids, cachedCount: 3, detailsSource);

        var result = await controller.GetBest(5, CancellationToken.None);

        var stories = GetStories(result);
        Assert.Equal([10, 9, 8, 7, 6], stories.Select(s => s.Score));
        Assert.Equal([6, 7], detailsSource.RequestedIds.OrderBy(id => id));
    }

    [Fact]
    public async Task GetBest_WhenRequestedCountExceedsAvailableStories_ReturnsAllAvailableStories()
    {
        int[] ids = [10, 9, 8, 7, 6];
        var detailsSource = new FakeStoryDetailsSource(ids);
        var controller = CreateController(ids, cachedCount: 3, detailsSource);

        var result = await controller.GetBest(100, CancellationToken.None);

        var stories = GetStories(result);
        Assert.Equal([10, 9, 8, 7, 6], stories.Select(s => s.Score));
    }
}
