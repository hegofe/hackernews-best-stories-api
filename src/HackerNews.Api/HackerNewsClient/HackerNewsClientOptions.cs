namespace HackerNews.Api.HackerNewsClient;

public sealed class HackerNewsClientOptions
{
    public const string SectionName = "HackerNewsClient";

    public string BaseAddress { get; set; } = "https://hacker-news.firebaseio.com/v0/";
}
