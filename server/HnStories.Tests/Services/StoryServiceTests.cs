using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using HnStories.Api.Models;
using HnStories.Api.Services;
using Moq;

namespace HnStories.Tests.Services;

public class StoryServiceTests
{
    private static StoryService Create(out Mock<IHnClient> hn)
    {
        hn = new Mock<IHnClient>();
        return new StoryService(hn.Object);
    }

    [Fact]
    public async Task Returns_paged_and_filtered()
    {
        var service = Create(out var hn);
        hn.Setup(x => x.GetNewStoryIdsAsync()).ReturnsAsync(Enumerable.Range(1, 50).Reverse().ToArray());
        hn.Setup(x => x.GetItemAsync(It.IsAny<int>())).ReturnsAsync((int id) => new HnItem
        {
            id = id,
            type = "story",
            title = id % 2 == 0 ? $"Even {id}" : $"Odd {id}",
            by = "tester",
            time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            url = id % 3 == 0 ? null : $"https://test.com/{id}"
        });

        var (items, total) = await service.GetNewestAsync("even", page: 1, pageSize: 10);
        total.Should().BeGreaterThan(0);
        items.Should().OnlyContain(s => s.Title.Contains("Even"));
        items.Should().HaveCount(10);
        items.Should().OnlyContain(s => s.Url != null && s.Url.Length > 0);
    }
}
