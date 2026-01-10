using System.Text.Json.Serialization;

namespace DreamShelf.API.Common.Dtos.Response;

public class ChapterListItemResponseDto
{
    public Guid Uuid { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    
    [JsonIgnore]
    public bool IsPublished { get; set; }
}