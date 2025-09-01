using Xunit;
using System.Net.Http.Json;
using FluentAssertions;
using HnStories.Api;
using HnStories.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HnStories.Tests.Api;

public class StoriesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StoriesEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IHnClient, FakeClient>();
            });
        });
    }

    [Fact]
    public async Task Get_returns_paged_result()
    {
        var client = _factory.CreateClient();
        var result = await client.GetFromJsonAsync<PagedResult>("/api/stories?page=1&pageSize=5");
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(5);
        result.Total.Should().Be(20);
    }

    private sealed record StoryDto(int Id, string Title, string? Url, string By, DateTimeOffset Time);
    private sealed record PagedResult(List<StoryDto> Items, int Page, int PageSize, int Total);

    private sealed class FakeClient : IHnClient
    {
        public Task<HnStories.Api.Models.HnItem?> GetItemAsync(int id) =>
            Task.FromResult<HnStories.Api.Models.HnItem?>(new()
            {
                id = id,
                type = "story",
                title = $"Title {id}",
                by = "fake",
                time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                url = id % 2 == 0 ? null : $"https://example.com/{id}"
            });

        public Task<IReadOnlyList<int>> GetNewStoryIdsAsync() =>
            Task.FromResult<IReadOnlyList<int>>(Enumerable.Range(1, 20).ToArray());
    }
}
