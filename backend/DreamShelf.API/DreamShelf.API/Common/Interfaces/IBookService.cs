using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Interfaces;

public interface IBookService
{
    Task<int> CreateBook(CreateBookDto bookRequestDto, Role role);
    Task<BookResponseDto?> GetBook(int id, bool isEdit, Role? role, int? authorId);
    Task<Result> UpdateBook(UpdateBookRequestDto bookRequestDto, int id, int authorId, Role role);
    Task<Result> DeleteBook(int bookId,  int authorIdClaim, Role role);
    Task<List<AdminPanelBook>> SearchBooks(string query);
    Task<List<BookCardResponseDto>> GetNewBooks();
    Task<List<BookCardResponseDto>> GetBooks(List<int> ids);
}