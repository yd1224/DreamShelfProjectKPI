using DreamShelf.API.Common.Dtos.Response.Response;

namespace DreamShelf.API.Common.Interfaces;

public interface IGenresService
{
    Task<List<GenreResponseDto>> GetGenres();
}