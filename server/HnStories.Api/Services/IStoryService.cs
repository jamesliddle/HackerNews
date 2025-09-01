using HnStories.Api.Models;

namespace HnStories.Api.Services;

public interface IStoryService
{
    Task<(IReadOnlyList<StoryDto> Items, int Total)> GetNewestAsync(
        string? query,
        int page,
        int pageSize);
}
