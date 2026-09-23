using HackerNews.Api.Contracts;

namespace HackerNews.Api.Tests.Fakes;

public static class StoryFactory
{
    public static StoryResponse Create(int id) => new(
        $"Title {id}",
        $"https://example.com/{id}",
        $"user{id}",
        DateTimeOffset.FromUnixTimeSeconds(1_570_887_781),
        id,
        id * 2);
}
