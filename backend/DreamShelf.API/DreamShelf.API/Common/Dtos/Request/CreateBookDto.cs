using DreamShelf.API.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Common.Dtos.Request;

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<int> Genres { get; set; } = new();
    public AgeRestriction AgeRestriction { get; set; }
    public string? CoverImageUrl { get; set; }
    [FromForm(Name = "document")]
    public IFormFile? DocumentFile { get; set; }
    public string? SplitBy { get; set; } = "h1";
    public BookStatus BookStatus { get; set; }
    public List<int> AuthorIds { get; set; }
}