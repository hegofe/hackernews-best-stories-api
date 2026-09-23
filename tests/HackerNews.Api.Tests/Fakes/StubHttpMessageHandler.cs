using System.Net;
using System.Text;

namespace HackerNews.Api.Tests.Fakes;

public sealed class StubHttpMessageHandler(Dictionary<string, string> responsesByPath) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri!.AbsolutePath.TrimStart('/');

        if (!responsesByPath.TryGetValue(path, out var json))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
