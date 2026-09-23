namespace HackerNews.Api.Caching;

public sealed class StoriesCacheOptions
{
    public const string SectionName = "StoriesCache";

    public int RefreshIntervalSeconds { get; set; } = 30;
    public int MaxCachedStories { get; set; } = 200;
    public int MaxDegreeOfParallelism { get; set; } = 20;
}
