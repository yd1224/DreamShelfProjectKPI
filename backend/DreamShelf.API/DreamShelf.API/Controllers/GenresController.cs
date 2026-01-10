using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
public class GenresController (IGenresService genresService) : ControllerBase
{
    private readonly IGenresService _genresService = genresService;
    
    [HttpGet("api/genres")]
    public async Task<ActionResult<List<GenreResponseDto>>> GetGenreNames()
    {
        var genres = await _genresService.GetGenres();
        return Ok(genres);
    }
}