using HnStories.Api.Models;

namespace HnStories.Api.Services;

public interface IHnClient
{
    Task<IReadOnlyList<int>> GetNewStoryIdsAsync();
    Task<HnItem?> GetItemAsync(int id);
}
