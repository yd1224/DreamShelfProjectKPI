using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Dtos.Response.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class ChapterService(ApplicationDbContext context, IImageService imageService): IChapterService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IImageService _imageService =  imageService;
    
    public async Task<ChapterResponseDto?> GetChapter(Guid chapterId, int bookId, int? userAuthorId, Role? role, bool isEditing)
    {
        var chapter = await _context.Chapters
            .Where(c => c.Uuid == chapterId)
            .Include(c => c.Book)
            .ThenInclude(b => b.Authors)
            .Select(c => new Chapter
            {
                Uuid =  c.Uuid,
                ChapterNumber = c.ChapterNumber,
                Title = c.Title,
                HTMLContent =  c.HTMLContent,
                IsPublished = c.IsPublished,
                LastUpdatedAt =  c.LastUpdatedAt,
                Book = new Book
                {
                    IsPublished = c.Book.IsPublished,
                    Authors = c.Book.Authors,
                    CoverImageUrl =  c.Book.CoverImageUrl,
                    Title =  c.Book.Title,
                    AddedBy = c.Book.AddedBy
                }
            })
            .FirstOrDefaultAsync();

        if (chapter is null)
            return null;

        bool isAuthor = chapter.Book.Authors.Any(a => a.Id == userAuthorId) ||
                        (chapter.Book.AddedBy == Role.Admin && role == Role.Admin);
        
        if (isEditing)
        {
            if (!isAuthor)
            {
                return null;
            }
        }
        else
        {
            if (!isAuthor && (!chapter.Book.IsPublished || !chapter.IsPublished))
            {
                return null;
            }
        }
        
        return new ChapterResponseDto
        {
            Uuid = chapter.Uuid,
            ChapterNumber = chapter.ChapterNumber,
            Title = chapter.Title,
            BookTitle = chapter.Book.Title,
            BookCoverImageUrl = chapter.Book.CoverImageUrl,
            HTMLContent = chapter.HTMLContent,
            IsPublished = chapter.IsPublished,
            LastUpdatedAt = chapter.LastUpdatedAt,
        };
    }
    
    public async Task<Result> UpdateChapter(ChapterRequestDto request, Guid chapterId, int bookId, int userAuthorId, Role role)
    {
        var existingChapter = await _context.Chapters
            .Include(c => c.Book)
            .ThenInclude(b => b.Authors)
            .FirstOrDefaultAsync(c => c.Uuid == chapterId);

        if (existingChapter == null)
            return new Result
            {
                isSuccess = false,
                Message = $"Chapter {chapterId} not found"
            };
        
        var isAuthor = existingChapter.Book.Authors.Any(a => a.Id == userAuthorId) ||
                       (existingChapter.Book.AddedBy == Role.Admin && role == Role.Admin);

        if (!isAuthor)
        {
            return new Result
            {
                isSuccess = false,
                Message = "Not authorized",
                ErrorType = ErrorType.Unauthorized
            };
        }

        
        
        var chapterNumberExists = await  _context.Chapters
            .Where(c => c.BookId == bookId && 
                        c.ChapterNumber == request.ChapterNumber).AnyAsync();

        if (chapterNumberExists && request.ChapterNumber != existingChapter.ChapterNumber)
        {
            return new Result
            {
                isSuccess = false,
                Message = $"Chapter number {request.ChapterNumber} is already in use"
            };
        }
        
        existingChapter.Title = request.Title;
        existingChapter.ChapterNumber = request.ChapterNumber;
        existingChapter.HTMLContent = request.HTMLContent;
        existingChapter.IsPublished = request.IsPublished;
        
        if (request.NewImageIds.Count > 0)
        {
            foreach (var image in request.NewImageIds)
            {
                await _context.BookImages.AddAsync(new BookImage
                {
                    BookId = bookId,
                    ImageId = image,
                    ChapterId = chapterId
                });
            }
        }
        
        await _context.SaveChangesAsync();

        return new Result
        {
            isSuccess = true,
        };
    }

    public async Task<Result> CreateChapter(ChapterRequestDto request, int bookId, int userAuthorId, Role role)
    {
        var book = await _context.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            return new Result
            {
                isSuccess = false,
                Message = $"Book {bookId} not found"
            };
        }
        
        var isAuthor = book.Authors.Any(a => a.Id == userAuthorId) ||
                       (book.AddedBy == Role.Admin && role == Role.Admin);
        
        if (!isAuthor)
        {
            return new Result
            {
                isSuccess = false,
                ErrorType = ErrorType.Unauthorized
            };
        }
        
        var isNumberTaken = await _context.Chapters
            .AnyAsync(c => c.BookId == bookId && c.ChapterNumber == request.ChapterNumber);

        if (isNumberTaken)
        {
            return new Result
            {
                isSuccess = false,
                Message = $"Chapter number {request.ChapterNumber} already exists for this book."
            };
        }
        
        var chapter = new Chapter
        {
            BookId = bookId,
            Title = request.Title,
            ChapterNumber = request.ChapterNumber,
            HTMLContent = request.HTMLContent,
            IsPublished = request.IsPublished,
        };

        await _context.Chapters.AddAsync(chapter);

        if (request.NewImageIds.Count > 0)
        {
            foreach (var image in request.NewImageIds)
            {
                await _context.BookImages.AddAsync(new BookImage
                {
                    BookId = bookId,
                    ImageId = image,
                    ChapterId = chapter.Uuid
                });
            }
        }
        
        await _context.SaveChangesAsync();

        return new Result
        {
            isSuccess = true,
        };
    }
    
    public async Task<Result> DeleteChapter(int bookId, Guid chapterId, int userAuthorId, Role role)
    {
        if (!await _context.Books.AnyAsync(b => b.Id == bookId &&
                                                (b.Authors.Any(a => a.Id == userAuthorId) ||
                                                 (b.AddedBy == Role.Admin && role == Role.Admin)
                                                 )))
        {
            return new Result
            {
                isSuccess = false,
                Message = "You do not have permission to delete this chapter",
                ErrorType = ErrorType.Unauthorized
            };
        }
        
        // Fetch the chapter along with its book and authors
        var chapter = await _context.Chapters
            .Where(c => c.Uuid == chapterId && c.BookId == bookId)
            .Include(c => c.BookImages)
            .FirstOrDefaultAsync();

        if (chapter == null)
        {
            return new Result
            {
                isSuccess = false,
                Message = $"Chapter {chapterId} not found",
                ErrorType = ErrorType.NotFound,
            };
        }

        // Remove the chapter from the database
        _context.Chapters.Remove(chapter);
        
        await _context.SaveChangesAsync();
        
        _imageService.DeleteImages(chapter.BookImages.Select(bi => bi.ImageId).ToList());

        return new Result
        {
            isSuccess = true,
            Message = "Chapter deleted successfully"
        };
    }

    
}