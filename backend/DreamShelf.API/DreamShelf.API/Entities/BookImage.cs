namespace DreamShelf.API.Entities;

public class BookImage
{
    public int BookId { get; set; }
    public string ImageId { get; set; }
    public Guid? ChapterId { get; set; }
    public Book Book { get; set; }
    public Chapter? Chapter { get; set; }
}