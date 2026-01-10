using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Common.Dtos.Request;

public class UpdateBookRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<int> Genres { get; set; } = [];
    public AgeRestriction AgeRestriction { get; set; }
    public string? CoverImageUrl { get; set; }
    public BookStatus BookStatus { get; set; }
    public bool IsPublished { get; set; }

    public List<int> AuthorIds { get; set; } = [];
}