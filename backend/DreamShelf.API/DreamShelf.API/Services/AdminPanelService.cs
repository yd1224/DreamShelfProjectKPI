using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class AdminPanelService(ApplicationDbContext context, IBookSearchService bookSearchService) : IAdminPanelService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IBookSearchService _bookSearchService = bookSearchService;
    private const int MaxPageSize = 5;
    
    public async Task<AdminPanelBooksResponseDto?> GetBooks(string roleClaim, int authorId, int page)
    {
        bool isAdmin = roleClaim == nameof(Role.Admin);
        
        var query = _context.Books.AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(b=>b.Authors.Any(a=>a.Id == authorId));
        }

        var booksCount = await query.CountAsync();

        int pageCount = (int)Math.Ceiling(booksCount / (double)MaxPageSize);

        if (page > pageCount)
        {
            return null;
        }
        
        var toSkip = (page - 1) * MaxPageSize;
        
        var books = await query.Select(b => new AdminPanelBook
        {
            Id = b.Id,
            Title = b.Title,
            Authors = b.Authors.Select(a => a.DisplayName).ToList(),
            PublicationYear = b.PublicationYear,
            BookStatus = b.BookStatus,
            LastUpdatedAt = b.LastUpdatedAt,
            IsPublished = b.IsPublished,
        })
        .Skip(toSkip)
        .Take(MaxPageSize) 
        .ToListAsync();

        return new AdminPanelBooksResponseDto
        {
            Books = books,
            HasNextPage = page != pageCount,
            HasPreviousPage = page > 1
        };
    }
    
    public async Task<AdminPanelBooksResponseDto> SearchBooks(string query, string roleClaim, int authorId, int page = 1)
    {
        // Lucene returns ranked book IDs
        var paginationResult = _bookSearchService.PaginationSearch(query, page);

        if (paginationResult.BookIds.Count == 0)
            return new AdminPanelBooksResponseDto();

        // Fetch all matching books in ONE SQL query
        var bookQuery = _context.Books
            .Where(b => paginationResult.BookIds.Contains(b.Id)); // filters only needed books

        if (roleClaim == nameof(Role.Author))
        {
            bookQuery = bookQuery.Where(b => b.Authors.Any(a => a.Id == authorId));
        }

        var books = await bookQuery.ToListAsync();
        // Dictionary for fast lookup
        var lookup = books.ToDictionary(b => b.Id);

        var results = new List<AdminPanelBook>();

        // Preserve Lucene ranking
        foreach (var id in paginationResult.BookIds)
        {
            if (!lookup.TryGetValue(id, out var book))
                continue; // skip missing or unpublished

            results.Add(new AdminPanelBook
            {
                Id = book.Id,
                Title = book.Title,
                BookStatus = book.BookStatus,
                Authors = book.Authors.Select(a => a.DisplayName).ToList(),
                PublicationYear =  book.PublicationYear,
                LastUpdatedAt = book.LastUpdatedAt,
                IsPublished = book.IsPublished,
            });
        }

        return new AdminPanelBooksResponseDto
        {
            HasNextPage = paginationResult.HasNextPage,
            HasPreviousPage =  paginationResult.HasPreviousPage,
            Books = results,
        };
    }

}