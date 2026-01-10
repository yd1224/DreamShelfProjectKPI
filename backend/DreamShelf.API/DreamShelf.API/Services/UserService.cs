using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using DreamShelf.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class UserService(
    ApplicationDbContext context,
    IAuthService authService,
    IAuthorSearchService authorSearch): IUserService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IAuthService _authService = authService;
    private readonly IAuthorSearchService _authSearchService = authorSearch;
    
    public async Task<bool> UserExists(string email)
    {   
        return await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<AccessTokenDto> RegisterUser(string email, string password, string deviceType)
    {
        var passwordHash = _authService.GeneratePasswordHash(password);

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash
        };
        
        var accessToken = await _authService.GenerateJwtToken(user);
        var refreshToken = _authService.GenerateRefreshToken();
        
        var userRefreshToken = new UserRefreshToken
        {
            Token = refreshToken.token,
            Expiration = refreshToken.expirationDate,
            DeviceInfo = deviceType
        };
        
        user.RefreshTokens.Add(userRefreshToken);
        
        //user id is set automatically as it is navigation property
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.token,
            RefreshTokenExpiration = refreshToken.expirationDate
        };
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<AccessTokenDto> LogInUser(User user, string deviceType)
    {
        var existingTokenForDeviceType = await _context.UserRefreshTokens
            .Where(urt => urt.UserId == user.Id && urt.DeviceInfo == deviceType)
            .FirstOrDefaultAsync();
        
        var accessToken = await _authService.GenerateJwtToken(user);
        var refreshToken = _authService.GenerateRefreshToken();
        
        if (existingTokenForDeviceType != null)
        {
            existingTokenForDeviceType.Token = refreshToken.token;
            existingTokenForDeviceType.Expiration = refreshToken.expirationDate;
        }
        else
        {
            var userRefreshToken = new UserRefreshToken
            {
                UserId = user.Id,
                Token = refreshToken.token,
                Expiration = refreshToken.expirationDate,
                DeviceInfo = deviceType
            };
            
            _context.UserRefreshTokens.Add(userRefreshToken);
        }
        
        await _context.SaveChangesAsync();
        
        return new AccessTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.token,
            RefreshTokenExpiration = refreshToken.expirationDate
        };
    }
    
    public async Task<AccessTokenDto> RefreshUser(UserRefreshToken userRefreshToken)
    {
        var newAccessToken = await _authService.GenerateJwtToken(userRefreshToken.User);
        var newRefreshToken = _authService.GenerateRefreshToken();
        
        userRefreshToken.Token = newRefreshToken.token;
        userRefreshToken.Expiration = newRefreshToken.expirationDate;

        await _context.SaveChangesAsync();
        
        return new AccessTokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.token,
            RefreshTokenExpiration = newRefreshToken.expirationDate
        };
    }
    
    public async Task<UserRefreshToken?> GetUserRefreshToken(string refreshToken)
    {
        return await _context.UserRefreshTokens.Include(urt=> urt.User)
            .FirstOrDefaultAsync(urt => urt.Token == refreshToken);
    }
    
    public async Task LogoutUser(int userId, string deviceType)
    {
        await _context.UserRefreshTokens
            .Where(urt => urt.UserId == userId && urt.DeviceInfo == deviceType)
            .ExecuteDeleteAsync();
    }

    public async Task<string> MakeAuthor(int id, MakeAuthorRequestDto request)
    {
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Pseudonym = request.Pseudonym,
            DisplayName = UserUtils.DisplayName(request.FirstName, request.LastName,  request.Pseudonym),
            UserId = id
        };
        
        await _context.Authors.AddAsync(author);

        var user = await _context.Users.FirstOrDefaultAsync(usr => usr.Id == id);
        
        user.Role = Role.Author;
        
        await _context.SaveChangesAsync();
        
        if (author.FirstName != null && author.LastName != null)
        {
            _authSearchService.AddToIndex(author.Id, $"{author.FirstName} {author.LastName}", author.Pseudonym);
        }
        else if (author.Pseudonym != null)
        {
            _authSearchService.AddToIndex(author.Id, null, author.Pseudonym);
        }
        
        var newAccessToken = await _authService.GenerateJwtToken(user);

        return newAccessToken;
    }
    
}