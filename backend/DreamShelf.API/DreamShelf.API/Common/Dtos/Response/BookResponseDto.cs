using System.Text.Json.Serialization;
using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Dtos.Response.Response;

public class BookResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<GenreResponseDto> Genres { get; set; } = new();
    public AgeRestriction AgeRestriction { get; set; }
    public List<AuthorListItemResponseDto> Authors { get; set; } = new();
    public List<ChapterListItemResponseDto> Chapters { get; set; } = new();
    public string? CoverImageUrl { get; set; }
    public int? PublicationYear { get; set; }
    public BookStatus BookStatus { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    
    [JsonIgnore]
    public bool IsPublished { get; set; }
    [JsonIgnore]
    public Role AddedBy  { get; set; }
}