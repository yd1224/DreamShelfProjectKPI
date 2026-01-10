using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DreamShelf.API.Services;

public class GenresService(ApplicationDbContext context,  IMemoryCache cache): IGenresService
{
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationDbContext _context = context;
    
    const string GenresCacheKey = "genreNamesCacheKey";
    
    public async Task<List<GenreResponseDto>> GetGenres()
    {
        if(_cache.TryGetValue(GenresCacheKey, out List<GenreResponseDto>? cachedGenreNames))
        {
            return cachedGenreNames ?? [];
        }   
        
        // Select only the Name property
        var genres = await _context.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreResponseDto
            {
                Id = g.Id,
                Name = g.Name
            }) .ToListAsync();
        
        _cache.Set(GenresCacheKey, genres, TimeSpan.FromDays(1));

        return genres;
    }
}