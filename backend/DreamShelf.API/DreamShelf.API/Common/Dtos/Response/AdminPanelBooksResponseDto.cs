using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Dtos.Response;

public class AdminPanelBooksResponseDto
{
    public List<AdminPanelBook> Books { get; set; } = new();
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class AdminPanelBook
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public int? PublicationYear { get; set; }
    public BookStatus BookStatus { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; }
    public string? CoverImageUrl { get; set; }
}