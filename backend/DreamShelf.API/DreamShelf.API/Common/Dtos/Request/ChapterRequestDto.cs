using System.Text.Json.Serialization;

namespace DreamShelf.API.Common.Dtos.Request;

public class ChapterRequestDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("chapterNumber")]
    public int ChapterNumber { get; set; }
    [JsonPropertyName("htmlContent")]
    public string HTMLContent { get; set; }
    [JsonPropertyName("isPublished")]
    public bool IsPublished { get; set; }

    public List<string> NewImageIds { get; set; } = [];
}