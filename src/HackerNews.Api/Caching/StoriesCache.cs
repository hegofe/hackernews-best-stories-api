namespace HackerNews.Api.Caching;

public sealed class StoriesCache : IStoriesCache
{
    private StoriesSnapshot _current = StoriesSnapshot.Empty;

    public StoriesSnapshot Current => Volatile.Read(ref _current);

    public void Update(StoriesSnapshot snapshot)
    {
        Volatile.Write(ref _current, snapshot);
    }
}
