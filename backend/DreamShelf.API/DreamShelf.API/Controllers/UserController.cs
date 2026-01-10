using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(
    IUserService userService,
    IAuthorService authorService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private  readonly IAuthorService _authorService = authorService;
    
    
    [HttpPost("become-author")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> BecomeAuthor([FromBody] MakeAuthorRequestDto request)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        
        if(!int.TryParse(userId, out var id))
            return BadRequest("Invalid user id");
        
        var token =  await _userService.MakeAuthor(id, request);
        
        return Ok(new { AccessToken = token });
    }
    
    [HttpPost("create-author")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorRequestDto request)
    {
        var author =  await _authorService.CreateAuthor(request);
        
        return Ok(new { author });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAuthors(string query)
    {
        var authors = await _authorService.SearchAuthors(query);
        return Ok(authors);
    }
    
    
}