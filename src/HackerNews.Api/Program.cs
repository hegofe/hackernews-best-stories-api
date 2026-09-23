using HackerNews.Api.BackgroundServices;
using HackerNews.Api.Caching;
using HackerNews.Api.HackerNewsClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<StoriesCacheOptions>(builder.Configuration.GetSection(StoriesCacheOptions.SectionName));

var hackerNewsClientOptions = builder.Configuration
    .GetSection(HackerNewsClientOptions.SectionName)
    .Get<HackerNewsClientOptions>() ?? new HackerNewsClientOptions();

builder.Services.AddHttpClient<IBestStoriesSource, HackerNewsBestStoriesSource>(client =>
{
    client.BaseAddress = new Uri(hackerNewsClientOptions.BaseAddress);
});

builder.Services.AddHttpClient<IStoryDetailsSource, HackerNewsStoryDetailsSource>(client =>
{
    client.BaseAddress = new Uri(hackerNewsClientOptions.BaseAddress);
});

builder.Services.AddSingleton<IStoriesCache, StoriesCache>();
builder.Services.AddHostedService<StoriesRefreshBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
