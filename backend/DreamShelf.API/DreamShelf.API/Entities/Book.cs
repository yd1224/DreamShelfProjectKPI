using DreamShelf.API.Enums;

namespace DreamShelf.API.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? CoverImageUrl { get; set; }
    public int? PublicationYear { get; set; }
    /// <summary>
    /// The year the book was originally published. 
    /// For user-generated content, this is the year the first chapter was published.
    /// </summary>
    public BookStatus BookStatus { get; set; }
    public bool IsPublished { get; set; } = false; // when author make it visible for others
    public AgeRestriction AgeRestriction { get; set; } = AgeRestriction.AllAges;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
    public Role AddedBy { get; set; }
    
    public ICollection<Author> Authors { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Chapter> Chapters { get; set; } = [];
    
    public ICollection<BookImage> BookImages { get; set; } = [];
    
}
// Адмін може додавати видаляти редагувати, якщо він додає книгу то вона автоматично IsPublished true; додавання-редагування-видалення книг через попапи в адмін панелі
// адмін панель для авторів з можливістю додавати редагувати та видаляти авторів через попапи
// dotnet ef database update
// dotnet ef migrations add UpdateProperties
//when delete book delete images in chapters in folder

// Give user abolity to write chapter number