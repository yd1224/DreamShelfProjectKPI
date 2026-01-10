using DreamShelf.API.Common.Dtos.Service;

namespace DreamShelf.API.Common.Interfaces;

public interface IDocumentParserService
{
    Task<List<ChapterServiceDto>> ParseAndSplitIntoChapters(IFormFile document, string splitBy, int bookId);
    Task<string> SaveChapterImages(string html, Guid chapterId, int bookId);
}