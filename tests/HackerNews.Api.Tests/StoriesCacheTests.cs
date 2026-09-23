using HackerNews.Api.Caching;
using HackerNews.Api.Tests.Fakes;

namespace HackerNews.Api.Tests;

public class StoriesCacheTests
{
    [Fact]
    public void Current_BeforeAnyUpdate_IsEmpty()
    {
        var cache = new StoriesCache();

        Assert.Empty(cache.Current.AllIds);
        Assert.Empty(cache.Current.CachedStories);
    }

    [Fact]
    public void Update_ReplacesCurrentSnapshot()
    {
        var cache = new StoriesCache();
        var snapshot = new StoriesSnapshot([2, 1], [StoryFactory.Create(2), StoryFactory.Create(1)]);

        cache.Update(snapshot);

        Assert.Same(snapshot, cache.Current);
    }
}
