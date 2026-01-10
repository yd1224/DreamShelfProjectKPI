using System.Security.Claims;
using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
[Route("api/admin-panel/books")]
public class AdminPanelController(IAdminPanelService adminPanelService) : ControllerBase
{
    private readonly IAdminPanelService _adminPanelService = adminPanelService;
    private string? RoleClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    private string? AuthorIdClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "authorId")?.Value;
    
    [Authorize(Roles = "Admin, Author")]
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery]int page)
    {
        if(!int.TryParse(AuthorIdClaim,out var authorId) || string.IsNullOrEmpty(RoleClaim))
            return BadRequest();
        
        var books = await _adminPanelService.GetBooks(RoleClaim, authorId, page);
        
        return Ok(books);
    }
    
    [Authorize(Roles = "Admin, Author")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchBooks([FromQuery] string query, [FromQuery] int page = 1)
    {
        if (page < 1 || string.IsNullOrEmpty(RoleClaim) || string.IsNullOrEmpty(query) || !int.TryParse(AuthorIdClaim, out var authorId))
        {
            return BadRequest();
        }

        var books = await _adminPanelService.SearchBooks(query, RoleClaim, authorId, page);
        
        return Ok(books);
    }
}