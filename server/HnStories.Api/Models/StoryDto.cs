namespace HnStories.Api.Models;

public record StoryDto(
    int Id,
    string Title,
    string? Url,
    string By,
    DateTimeOffset Time
);
