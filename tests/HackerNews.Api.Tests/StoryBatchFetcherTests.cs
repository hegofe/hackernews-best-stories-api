using HackerNews.Api.Caching;
using HackerNews.Api.Tests.Fakes;

namespace HackerNews.Api.Tests;

public class StoryBatchFetcherTests
{
    [Fact]
    public async Task FetchAsync_ReturnsStoriesInTheSameOrderAsTheRequestedIds()
    {
        int[] ids = [50, 40, 30, 20, 10];
        var detailsSource = new FakeStoryDetailsSource(ids);

        var stories = await StoryBatchFetcher.FetchAsync(detailsSource, ids, 4, CancellationToken.None);

        Assert.Equal(ids, stories.Select(s => s.Score));
    }

    [Fact]
    public async Task FetchAsync_SkipsIdsThatHaveNoStory()
    {
        int[] ids = [50, 40, 30];
        var detailsSource = new FakeStoryDetailsSource([50, 30]);

        var stories = await StoryBatchFetcher.FetchAsync(detailsSource, ids, 4, CancellationToken.None);

        Assert.Equal([50, 30], stories.Select(s => s.Score));
    }
}
