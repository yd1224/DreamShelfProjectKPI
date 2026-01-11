using DreamShelf.API.Enums;

namespace DreamShelf.API.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }
    public int? PublicationYear { get; set; }
    public BookStatus BookStatus { get; set; }
    public bool IsPublished { get; set; } = false;
    public AgeRestriction AgeRestriction { get; set; } = AgeRestriction.AllAges;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public Role AddedBy { get; set; }
    
    public ICollection<Author> Authors { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Chapter> Chapters { get; set; } = [];
    
    public ICollection<BookImage> BookImages { get; set; } = [];
    
}
// dotnet ef database update
// dotnet ef migrations add UpdateProperties