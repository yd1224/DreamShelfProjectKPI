namespace DreamShelf.API.Common.Dtos.Response;

public class BookCardResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public List<string> Authors { get; set; } = new();
    public string? CoverImageUrl { get; set; }
}