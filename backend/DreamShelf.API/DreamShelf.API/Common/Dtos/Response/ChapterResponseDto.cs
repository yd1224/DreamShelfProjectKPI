namespace DreamShelf.API.Common.Dtos.Response.Response;

public class ChapterResponseDto
{
    public Guid Uuid { get; set; }
    public string Title { get; set; }
    public string BookTitle { get; set; }
    public string? BookCoverImageUrl { get; set; }
    public int ChapterNumber { get; set; }
    public string HTMLContent { get; set; }
    public bool IsPublished { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}