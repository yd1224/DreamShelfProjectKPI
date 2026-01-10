using System.ComponentModel.DataAnnotations;

namespace DreamShelf.API.Entities;

public class Author
{
    public int Id { get; set; }
    [StringLength(100)]
    public string? FirstName { get; set; }
    [StringLength(100)]
    public string? LastName { get; set; }
    [StringLength(100)]
    public string? Pseudonym { get; set; }
    [StringLength(300)]
    public string DisplayName { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }
    public User? User { get; set; }
    public ICollection<Book> Books { get; set; } = [];
    //first, las name or pseudonym is required
}