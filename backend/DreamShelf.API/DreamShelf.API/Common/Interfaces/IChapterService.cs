using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Interfaces;

public interface IChapterService
{
    Task<ChapterResponseDto?> GetChapter(Guid chapterId, int bookId, int? userAuthorId, Role? role, bool isEditing);
    Task<Result> UpdateChapter(ChapterRequestDto request, Guid chapterId, int bookId,  int userAuthorId, Role role);
    Task<Result> DeleteChapter(int bookId, Guid chapterId, int userAuthorId, Role role);
    Task<Result> CreateChapter(ChapterRequestDto request, int bookId, int userAuthorId, Role role);
}