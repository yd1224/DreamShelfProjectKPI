using System.Security.Claims;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[Route("api/images")]
public class ImageController (IImageService imageService) : ControllerBase
{
    
    private readonly  IImageService _imageService = imageService;
    
    private string? RoleClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    private string? AuthorIdClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "authorId")?.Value;

    
    [Authorize(Roles = "Admin, Author")]
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile image)
    {
        if (image.Length == 0)
        {
            return BadRequest();
        }
        var imageUrl = await _imageService.SaveImageAsync(image);
        return Ok(imageUrl);
    }
    
    [Authorize(Roles = "Admin, Author")]
    [HttpPost("delete")]
    public async Task<ActionResult<string>> DeleteImage([FromBody] List<string> ids)
    {
        if (!int.TryParse(AuthorIdClaim, out var authorId) || !Enum.TryParse(RoleClaim, out Role role))
        {
            return BadRequest();
        }
        
        foreach (var id in ids)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            
            await _imageService.DeleteImageAsync(id, authorId, role);
        }
        
        return Ok();
    }
}