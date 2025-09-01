using System.Net.Http.Json;
using HnStories.Api.Models;

namespace HnStories.Api.Services;

public sealed class HnClient(HttpClient http) : IHnClient
{
    public async Task<IReadOnlyList<int>> GetNewStoryIdsAsync()
    {
        var ids = await http.GetFromJsonAsync<int[]>("newstories.json") 
                  ?? Array.Empty<int>();
        return ids;
    }

    public Task<HnItem?> GetItemAsync(int id) =>
        http.GetFromJsonAsync<HnItem>($"item/{id}.json");
}
