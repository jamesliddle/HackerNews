using System.Collections.Concurrent;
using HnStories.Api.Models;

namespace HnStories.Api.Services;

public sealed class StoryService(IHnClient hn) : IStoryService
{

    public async Task<(IReadOnlyList<StoryDto> Items, int Total)> GetNewestAsync(
        string? query, int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0 || pageSize > 100) pageSize = 20;
        query = (query ?? string.Empty).Trim();

        var ids = await hn.GetNewStoryIdsAsync();
        var start = (page - 1) * pageSize;

        var bag = new ConcurrentBag<StoryDto>();
        var tasks = ids.Select(async id =>
        {
            var item = await hn.GetItemAsync(id);

            if (item is null || item.type != "story" || string.IsNullOrWhiteSpace(item.title))
                return;

            var dto = new StoryDto(
                item.id,
                item.title!,
                string.IsNullOrWhiteSpace(item.url) ? $"https://news.ycombinator.com/item?id={item.id}" : item.url,
                item.by ?? "unknown",
                DateTimeOffset.FromUnixTimeSeconds(item.time)
            );
            if (string.IsNullOrEmpty(query) || dto.Title.Contains(query, StringComparison.OrdinalIgnoreCase) || dto.By.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                bag.Add(dto);
            }
        });
        await Task.WhenAll(tasks);

        var ordered = bag.OrderByDescending(x => x.Time).ToList();
        var total = ordered.Count;
        var pageItems = ordered.Skip(start).Take(pageSize).ToList();

        return (pageItems, total);
    }
}
