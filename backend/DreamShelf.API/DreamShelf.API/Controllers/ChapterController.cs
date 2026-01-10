using System.Security.Claims;
using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
[Route("api/chapters")]
public class ChapterController(IChapterService chapterService) : ControllerBase
{
    private readonly IChapterService _chapterService =  chapterService;
    
    private string? RoleClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    private string? AuthorIdClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "authorId")?.Value;

    /// <summary>
    /// Gets chapter information
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="id"></param>
    /// <param name="isEdit"></param>
    /// <returns></returns>
    [HttpGet("{bookId:int}/{id:guid}")]
    public async Task<IActionResult> GetChapter(int bookId, Guid id, bool isEdit)
    {
        int? authorId = int.TryParse(AuthorIdClaim, out var parsedId) ? parsedId : null;
        Role? role = Enum.TryParse(RoleClaim, out Role roleClaim) ? roleClaim : null;
        
        var chapter = await _chapterService.GetChapter(id, bookId, authorId, role, isEditing: isEdit);
    
        return chapter is null ? NotFound() : Ok(chapter);
    }
    
    /// <summary>
    /// Updates chapter
    /// </summary>
    /// <param name="request"></param>
    /// <param name="bookId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpPut("{bookId:int}/{id:guid}")]
    public async Task<IActionResult> UpdateChapter([FromBody] ChapterRequestDto request,  int bookId, Guid id)
    {
        int? authorId = int.TryParse(AuthorIdClaim, out var parsedId) ? parsedId : null;
        Role? role = Enum.TryParse(RoleClaim, out Role roleClaim) ? roleClaim : null;

        if (authorId == null || role == null)
        {
            return BadRequest();
        }
        var result = await _chapterService.UpdateChapter(request, id, bookId,  authorId.Value, role.Value);

        if (!result.isSuccess)
        {
            switch (result.ErrorType)
            {
                case ErrorType.NotFound:
                    return NotFound(result.Message);
                case ErrorType.Unauthorized:
                    return Unauthorized(result.Message);
                default:
                    return BadRequest(result.Message);
            }
        }
        
        return Ok();
    }
    
    
    /// <summary>
    /// Deletes a chapter
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bookId"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpDelete("{bookId:int}/{id:guid}")]
    public async Task<IActionResult> DeleteChapter(Guid id, int  bookId)
    {
        int? authorId = int.TryParse(AuthorIdClaim, out var parsedId) ? parsedId : null;
        Role? role = Enum.TryParse(RoleClaim, out Role roleClaim) ? roleClaim : null;
        
        if (authorId == null || role == null)
        {
            return BadRequest();
        }
        
        var result = await _chapterService.DeleteChapter(bookId, id, authorId.Value, role.Value);

        if (!result.isSuccess)
        {
            switch (result.ErrorType)
            {
                case ErrorType.NotFound:
                    return NotFound(result.Message);
                case ErrorType.Unauthorized:
                    return Unauthorized(result.Message);
                default:
                    return BadRequest(result.Message);
            }
        }
        
        return Ok();
    }
    
    /// <summary>
    /// Creates a chapter
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpPost("{bookId:int}")]
    public async Task<IActionResult> CreateChapter(
        int bookId,
        [FromBody] ChapterRequestDto request)
    {
        int? authorId = int.TryParse(AuthorIdClaim, out var parsedId) ? parsedId : null;
        Role? role = Enum.TryParse(RoleClaim, out Role roleClaim) ? roleClaim : null;

        if (authorId == null || role == null)
        {
            return BadRequest();
        }
        
        var result = await _chapterService.CreateChapter(request, bookId, authorId.Value, role.Value);

        if (!result.isSuccess)
        {
            switch (result.ErrorType)
            {
                case ErrorType.NotFound:
                    return NotFound(result.Message);
                case ErrorType.Unauthorized:
                    return Unauthorized(result.Message);
                default:
                    return BadRequest(result.Message);
            }
        }
        
        return Ok();
    }

}