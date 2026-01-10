using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;

namespace DreamShelf.API.Common.Interfaces;

public interface IAuthorService
{
    Task<CreateAuthorResponseDto> CreateAuthor(CreateAuthorRequestDto request);
    Task<List<AuthorResponseDto>> SearchAuthors(string query);
}