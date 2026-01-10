namespace DreamShelf.API.Common.Dtos.Service;

public class ChapterServiceDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}