using HackerNews.Api.HackerNewsClient;
using HackerNews.Api.Tests.Fakes;

namespace HackerNews.Api.Tests;

public class HackerNewsSourcesTests
{
    private static HttpClient CreateClient(Dictionary<string, string> responsesByPath) =>
        new(new StubHttpMessageHandler(responsesByPath))
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };

    [Fact]
    public async Task GetBestStoryIdsAsync_ReturnsIdsInTheOrderReceived()
    {
        var client = CreateClient(new() { ["v0/beststories.json"] = "[30,10,20]" });
        var source = new HackerNewsBestStoriesSource(client);

        var ids = await source.GetBestStoryIdsAsync(CancellationToken.None);

        Assert.Equal([30, 10, 20], ids);
    }

    [Fact]
    public async Task GetStoryAsync_MapsHackerNewsItemToStoryResponse()
    {
        const string json = """
            {
                "by": "ismaildonmez",
                "descendants": 572,
                "id": 21233041,
                "score": 1716,
                "time": 1570887781,
                "title": "A uBlock Origin update was rejected from the Chrome Web Store",
                "type": "story",
                "url": "https://github.com/uBlockOrigin/uBlock-issues/issues/745"
            }
            """;
        var client = CreateClient(new() { ["v0/item/21233041.json"] = json });
        var source = new HackerNewsStoryDetailsSource(client);

        var story = await source.GetStoryAsync(21233041, CancellationToken.None);

        Assert.NotNull(story);
        Assert.Equal("A uBlock Origin update was rejected from the Chrome Web Store", story.Title);
        Assert.Equal("https://github.com/uBlockOrigin/uBlock-issues/issues/745", story.Uri);
        Assert.Equal("ismaildonmez", story.PostedBy);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1570887781), story.Time);
        Assert.Equal(1716, story.Score);
        Assert.Equal(572, story.CommentCount);
    }

    [Fact]
    public async Task GetStoryAsync_WhenItemIsDeleted_ReturnsNull()
    {
        const string json = """{ "id": 1, "type": "story", "deleted": true, "time": 1570887781 }""";
        var client = CreateClient(new() { ["v0/item/1.json"] = json });
        var source = new HackerNewsStoryDetailsSource(client);

        var story = await source.GetStoryAsync(1, CancellationToken.None);

        Assert.Null(story);
    }
}
