using DreamShelf.API.Common.Dtos.Response;

namespace DreamShelf.API.Common.Interfaces;

public interface IAdminPanelService
{
    Task<AdminPanelBooksResponseDto?> GetBooks(string roleClaim, int authorId, int page);
    Task<AdminPanelBooksResponseDto> SearchBooks(string query, string roleClaim, int authorId, int page = 1);
}