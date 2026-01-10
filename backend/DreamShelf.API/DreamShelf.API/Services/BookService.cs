using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class BookService(
    ApplicationDbContext context,
    IDocumentParserService documentParserService,
    IImageService imageService,
    IBookSearchService bookSearchService): IBookService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IDocumentParserService _documentParserService = documentParserService;
    private readonly IImageService _imageService = imageService;
    private readonly IBookSearchService _bookSearchService = bookSearchService;
    
    public async Task<int> CreateBook(CreateBookDto bookRequestDto, Role role)
    {
        var genres = await _context.Genres
            .Where(g => bookRequestDto.Genres.Contains(g.Id))
            .ToListAsync();
        
        var authors = await _context.Authors
            .Where(a => bookRequestDto.AuthorIds.Contains(a.Id)).ToListAsync();


        var book = new Book
        {
            Title = bookRequestDto.Title,
            Description = bookRequestDto.Description,
            AgeRestriction = bookRequestDto.AgeRestriction,
            Genres = genres,
            Authors = authors,
            CoverImageUrl =  bookRequestDto.CoverImageUrl,
            BookStatus =  bookRequestDto.BookStatus,
            AddedBy = role
        };
        
        await _context.Books.AddAsync(book);
        
        await _context.SaveChangesAsync();
        
        if (bookRequestDto.DocumentFile != null && bookRequestDto.SplitBy != null)
        {
            try
            {
                var chapters = await _documentParserService.ParseAndSplitIntoChapters(bookRequestDto.DocumentFile,
                    bookRequestDto.SplitBy, book.Id);

                List<Chapter> chapterList = chapters.Select((c, i) => new Chapter
                {
                    BookId =  book.Id,
                    Title = c.Title,
                    HTMLContent = c.Content,
                    ChapterNumber = i + 1
                }).ToList();
                
                await _context.Chapters.AddRangeAsync(chapterList);
                
                foreach (var chapter in chapterList)
                {
                    chapter.HTMLContent =
                        await _documentParserService.SaveChapterImages(chapter.HTMLContent, chapter.Uuid, book.Id);
                }
                
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await _context.Books
                    .Where(b => b.Id == book.Id)
                    .ExecuteDeleteAsync();
            }
        }
        
        _bookSearchService.AddToIndex(book.Id, book.Title);


        if (book.CoverImageUrl == null) 
            return book.Id;
        
        
        await _context.BookImages.AddAsync(new BookImage
        {
            ImageId = book.CoverImageUrl,
            BookId = book.Id
        });
            
        await _context.SaveChangesAsync();
        
        return book.Id;
    }

    public async Task<BookResponseDto?> GetBook(int id, bool isEdit, Role? role, int? authorId)
    {
        var book= await _context.Books
            .Select(b=>new BookResponseDto
        {
            Id = b.Id,
            Title = b.Title,
            Description = b.Description,
            CoverImageUrl = b.CoverImageUrl,
            AgeRestriction = b.AgeRestriction,
            PublicationYear = b.PublicationYear,
            BookStatus = b.BookStatus,
            LastUpdatedAt =  b.LastUpdatedAt,
            IsPublished = b.IsPublished,
            AddedBy =  b.AddedBy,
            Genres  = b.Genres.Select(g=> new GenreResponseDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList(),
            Authors = b.Authors.Select(a => new AuthorListItemResponseDto
            {
                Id = a.Id,
                DisplayName = a.DisplayName
            }).ToList(),
            Chapters = b.Chapters.Select(c=> new ChapterListItemResponseDto
            {
                Uuid = c.Uuid,
                Title = c.Title,
                ChapterNumber = c.ChapterNumber,
                LastUpdatedAt = c.LastUpdatedAt,
                IsPublished = c.IsPublished,
            }).ToList()
        }).FirstOrDefaultAsync(b=> b.Id == id);

        if (book is null)
            return null;
        
        bool isAuthor = book.Authors.Any(b => b.Id == authorId) || 
                        (book.AddedBy == Role.Admin && role == Role.Admin);
        
        if (!isEdit)
        {
            if (!isAuthor && !book.IsPublished)
                return null;
            else if (!isAuthor && book.IsPublished)
            {
                book.Chapters = book.Chapters.Where(c => c.IsPublished).ToList();
            }
        }
        else
        {
            if (!isAuthor)
            {
                return null;
            }
        }
        
        return book;
    }
    
    public async Task<Result> UpdateBook(UpdateBookRequestDto request, int bookId, int authorId, Role role)
    {
        var existingBook = await  _context.Books
            .Include(g => g.Genres)
            .Include(b => b.Authors)  
            .Where(b => b.Id == bookId).FirstOrDefaultAsync();

        if (existingBook == null)
            return new Result
            {
                isSuccess = false,
                Message = $"Book {bookId} not found"
            };
        
  
        var isAuthor = existingBook.Authors.Any(a => a.Id == authorId) ||
                       (role == Role.Admin && existingBook.AddedBy == Role.Admin);

        if (!isAuthor)
            return new Result
            {
                isSuccess = false,
                ErrorType = ErrorType.Unauthorized
            };

        List<Genre> genres = [];
        
        if (request.Genres.Count > 0)
        {
            genres = await _context.Genres
                .Where(g => request.Genres.Contains(g.Id)).ToListAsync();
        }

        List<Author> authors = [];
        
        if (request.AuthorIds.Count > 0)
        {
            authors = await  _context.Authors
                .Where(g => request.AuthorIds.Contains(g.Id)).ToListAsync();
        }
        
        if (request.CoverImageUrl != null && request.CoverImageUrl != existingBook.CoverImageUrl)
        {
            await _context.BookImages.AddAsync(new BookImage
            {
                ImageId = request.CoverImageUrl,
                BookId = bookId
            });
        }
        
        existingBook.Title = request.Title;
        existingBook.Description = request.Description;
        existingBook.AgeRestriction = request.AgeRestriction;
        existingBook.CoverImageUrl = request.CoverImageUrl;
        existingBook.IsPublished = request.IsPublished;
        existingBook.BookStatus = request.BookStatus;
        existingBook.Genres = genres;
        existingBook.Authors = authors;
        
        await _context.SaveChangesAsync();
        
        _bookSearchService.AddToIndex(existingBook.Id, existingBook.Title);

        return new Result
        {
            isSuccess = true,
        };
    }

    public async Task<Result> DeleteBook(int bookId, int authorIdClaim, Role role)
    {
        bool isAdmin = role == Role.Admin;
        Book? book = null;
        
        if (isAdmin)
        {
            book = await _context.Books
                .Include(b => b.BookImages)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }
        else
        {
            book = await _context.Books
                .Include(b => b.BookImages)
                .FirstOrDefaultAsync(b => b.Id == bookId && 
                                          b.Authors.Any(a => a.Id == authorIdClaim)); 
        }

        if (book == null)
        {
            return new Result
            {
                isSuccess = false
            };
        }
        
        await _context.Books
            .Where(b => b.Id == bookId)
            .ExecuteDeleteAsync();
        
        await _context.SaveChangesAsync();
        
        _imageService.DeleteImages(book.BookImages.Select(bi => bi.ImageId).ToList());
        
        _bookSearchService.RemoveFromIndex(book.Id);

        return new Result
        {
            isSuccess = true
        };
    }

    public async Task<List<AdminPanelBook>> SearchBooks(string query)
    {
        var bookIds = _bookSearchService.Search(query);  // List<int>

        if (!bookIds.Any())
            return new List<AdminPanelBook>();

        // Fetch all books in ONE query
        var books = await _context.Books
            .Where(b => bookIds.Contains(b.Id) && b.IsPublished)
            .Include(b => b.Authors)
            .ToListAsync();

        // Convert to dictionary for fast lookup
        var dict = books.ToDictionary(b => b.Id);

        List<AdminPanelBook> results = new();

        // Preserve Lucene ranking order
        foreach (var id in bookIds)
        {
            if (!dict.TryGetValue(id, out var book))
                continue;

            results.Add(new AdminPanelBook
            {
                Id = book.Id,
                Title = book.Title,
                BookStatus = book.BookStatus,
                Authors = book.Authors.Select(a => a.DisplayName).ToList(),
                PublicationYear =  book.PublicationYear,
                LastUpdatedAt = book.LastUpdatedAt,
                IsPublished = book.IsPublished,
                CoverImageUrl = book.CoverImageUrl,
            });
        }

        return results;
    }

    public async Task<List<BookCardResponseDto>> GetNewBooks()
    {
        var books = await _context.Books
            .Where(b => b.IsPublished)
            .OrderByDescending(b => b.AddedAt)
            .Take(20)
            .Select(b => new BookCardResponseDto
            {
                Id = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl,
                Authors = b.Authors.Select(a => a.DisplayName).ToList(),
                Genres = b.Genres.Select(g => g.Name).ToList(),
            })
            .ToListAsync();

        return books;
    }
    
    public async Task<List<BookCardResponseDto>> GetBooks(List<int> ids)
    {
        var books = await _context.Books
            .Where(b => b.IsPublished &&  ids.Contains(b.Id))
            .Select(b => new BookCardResponseDto
            {
                Id = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl,
                Authors = b.Authors.Select(a => a.DisplayName).ToList(),
                Genres = b.Genres.Select(g => g.Name).ToList(),
            })
            .ToListAsync();

        return books;
    }

}