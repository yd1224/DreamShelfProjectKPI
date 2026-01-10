using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamShelf.API.Entities;

public class Chapter
{
    [Key]
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public string HTMLContent { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public int BookId { get; set; }
    public Book Book { get; set; }

    public ICollection<BookImage> BookImages { get; set; } = [];
    // think about images
}