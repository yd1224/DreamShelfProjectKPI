using DreamShelf.API.Common.Dtos.Response;

namespace DreamShelf.API.Common.Interfaces;

public interface IAuthorSearchService
{
    void AddToIndex(int id, string? fullName, string? pseudonym);
    List<AuthorResponseDto> Search(string term);
}