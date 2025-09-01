namespace HnStories.Api.Models;

public class HnItem
{
    public int id { get; set; }
    public string? type { get; set; }
    public string? by { get; set; }
    public long time { get; set; }
    public string? title { get; set; }
    public string? url { get; set; }
}
