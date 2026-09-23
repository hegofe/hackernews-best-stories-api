using System.Net.Http.Json;

using var client = new HttpClient
{
    BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
};

var ids = await client.GetFromJsonAsync<int[]>("beststories.json") ?? [];

var scores = new int?[ids.Length];
var allKids = new HashSet<int>();
var kidsLock = new object();

await Parallel.ForEachAsync(
    Enumerable.Range(0, ids.Length),
    new ParallelOptions { MaxDegreeOfParallelism = 20 },
    async (index, cancellationToken) =>
    {
        var item = await client.GetFromJsonAsync<Item>($"item/{ids[index]}.json", cancellationToken);
        scores[index] = item?.Score;

        if (item?.Kids is { } kids)
        {
            lock (kidsLock)
            {
                allKids.UnionWith(kids);
            }
        }
    });

for (var i = 0; i < ids.Length; i++)
{
    Console.WriteLine($"{i,4} id={ids[i],-10} score={scores[i]}");
}

var isDescending = true;
for (var i = 1; i < scores.Length; i++)
{
    if (scores[i - 1] is null || scores[i] is null)
    {
        continue;
    }

    if (scores[i] > scores[i - 1])
    {
        isDescending = false;
        Console.WriteLine($"Order break at index {i}: id={ids[i - 1]} score={scores[i - 1]} -> id={ids[i]} score={scores[i]}");
    }
}

Console.WriteLine(isDescending
    ? "Scores are in descending order."
    : "Scores are NOT in descending order.");

var lines = ids.Select((id, i) => $"{id},{scores[i]}");
await File.WriteAllLinesAsync(@"C:\Users\hecto\Desktop\results.txt", lines);

var idSet = ids.ToHashSet();
var kidsNotInIds = allKids.Where(kid => !idSet.Contains(kid)).ToList();

Console.WriteLine($"Total distinct kid ids: {allKids.Count}");
Console.WriteLine(kidsNotInIds.Count == 0
    ? "All kid ids are present in the beststories id list."
    : $"{kidsNotInIds.Count} kid ids are NOT present in the beststories id list.");

record Item(int Id, int? Score, int[]? Kids);
