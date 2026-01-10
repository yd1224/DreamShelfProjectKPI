using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Entities;
using DreamShelf.API.Persistence;
using DreamShelf.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class AuthorService (
    ApplicationDbContext context,
    IAuthorSearchService authorSearchService) : IAuthorService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IAuthorSearchService _authorSearchService = authorSearchService;
    
    
        
    public async Task<CreateAuthorResponseDto> CreateAuthor(CreateAuthorRequestDto request)
    {
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Pseudonym = request.Pseudonym,
            DeathDate =  request.DeathDate,
            DisplayName = UserUtils.DisplayName(request.FirstName, request.LastName, request.Pseudonym)
        };
        
        await _context.Authors.AddAsync(author);
        
        await _context.SaveChangesAsync();

        if (author.FirstName != null && author.LastName != null)
        {
            _authorSearchService.AddToIndex(author.Id, $"{author.FirstName} {author.LastName}", author.Pseudonym);
        }
        else if (author.Pseudonym != null)
        {
            _authorSearchService.AddToIndex(author.Id, null, author.Pseudonym);
        }
        

        var authorResponse = new CreateAuthorResponseDto
        {
            Id = author.Id,
            DisplayName = author.DisplayName
        };

        return authorResponse;
    }

    public async Task<List<AuthorResponseDto>> SearchAuthors(string query)
    {
        var authorsSearchResult = _authorSearchService.Search(query);
        
        List<int> authorIds = authorsSearchResult.Select(x => x.Id).ToList();
        
        var authors = await _context.Authors.Where(a => authorIds.Contains(a.Id))
            .Select(a => new AuthorResponseDto
            {
                Id = a.Id,
                DisplayName = a.DisplayName
            }).ToListAsync();
        
        return authors;
    }
}