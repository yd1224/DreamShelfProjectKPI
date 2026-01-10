using System.Security.Claims;
using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Enums;
using DreamShelf.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamShelf.API.Controllers;

[ApiController]
[Route("api/book")]
public class BookController (IBookService bookService) : ControllerBase
{
    private readonly IBookService _bookService = bookService;
    private string? RoleClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    private string? AuthorIdClaim => HttpContext.User.Claims.FirstOrDefault(c => c.Type == "authorId")?.Value;
    
    /// <summary>
    /// Creates a book
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateBook([FromForm] CreateBookDto request)
    {
        if (request.DocumentFile != null)
        {
            string fileFormat = Path.GetExtension(request.DocumentFile.FileName).ToLower();
        
            if (fileFormat != ".docx")
                return BadRequest();
        }
        
        if (string.IsNullOrEmpty(RoleClaim))
        {
            return BadRequest();
        }
        
        Role role = Enum.Parse<Role>(RoleClaim);

        var bookId = await _bookService.CreateBook(request, role);
        
        return Ok(new { BookId = bookId });
    }

    /// <summary>
    /// Gets book information
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isEdit"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBook(int id, bool isEdit)
    {
        Role? role = !string.IsNullOrEmpty(RoleClaim) ? Enum.Parse<Role>(RoleClaim) : null;

        int? authorId = null;

        if (AuthorIdClaim != null && int.TryParse(AuthorIdClaim, out var parsedAuthorId))
        {
            authorId = parsedAuthorId;
        }

        var book = await _bookService.GetBook(id, isEdit, role, authorId);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    /// <summary>
    /// Updates book
    /// </summary>
    /// <param name="request"></param>
    /// <param name="bookId"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpPut("update/{bookId:int}")]
    public async Task<IActionResult> UpdateBook([FromBody] UpdateBookRequestDto request,  int bookId)
    {
        if (!Enum.TryParse(RoleClaim, out Role role) || !int.TryParse(AuthorIdClaim, out var authorId))
        {
            return  BadRequest();
        }
        
        
        var result = await _bookService.UpdateBook(request, bookId,  authorId, role);

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
    /// Deletes book
    /// </summary>
    /// <param name="bookId"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin, Author")]
    [HttpDelete("delete/{bookId:int}")]
    public async Task<IActionResult> DeleteBook(int bookId)
    {
        
        if(!int.TryParse(AuthorIdClaim, out var authorId) || !Enum.TryParse(RoleClaim, out Role role))
            return BadRequest();
        
        var result = await _bookService.DeleteBook(bookId,  authorId, role);
    
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
    /// Searches book by name
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(string query)
    {
        var books = await _bookService.SearchBooks(query);
        
        return  Ok(books);
    }
    
    /// <summary>
    /// Returns books by ids
    /// </summary>
    /// <param name="ids">List of ids passed by frontend</param>
    /// <returns></returns>
    [HttpPost("books")]
    public async Task<IActionResult> GetBooks([FromBody] List<int> ids)
    {
        var books = await _bookService.GetBooks(ids);
        
        return  Ok(books);
    }
    
    /// <summary>
    /// Gets newest books
    /// </summary>
    /// <returns></returns>
    [HttpGet("new")]
    public async Task<IActionResult> GetNewBooks()
    {
        var books = await _bookService.GetNewBooks();
        
        return  Ok(books);
    }
}