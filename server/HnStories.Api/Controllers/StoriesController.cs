using HnStories.Api.Models;
using HnStories.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HnStories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoriesController(IStoryService stories) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<StoryDto>>> Get(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var (items, total) = await stories.GetNewestAsync(query, page, pageSize);
        return Ok(new PagedResult<StoryDto>(items, page, pageSize, total));
    }
}

public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
