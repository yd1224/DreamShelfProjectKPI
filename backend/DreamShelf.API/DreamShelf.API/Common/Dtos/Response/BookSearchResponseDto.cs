using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Dtos.Response;

public class BookSearchResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
}