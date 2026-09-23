namespace HackerNews.Api.Caching;

public interface IStoriesCache
{
    StoriesSnapshot Current { get; }
    void Update(StoriesSnapshot snapshot);
}
