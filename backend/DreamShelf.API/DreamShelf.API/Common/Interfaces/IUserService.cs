using DreamShelf.API.Common.Dtos.Request;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Entities;

namespace DreamShelf.API.Common.Interfaces;

public interface IUserService
{
    Task<bool> UserExists(string email);
    Task<AccessTokenDto> RegisterUser(string email, string password, string deviceType);
    Task<User?> GetUserByEmail(string email);
    Task<AccessTokenDto> LogInUser(User user, string deviceType);
    Task LogoutUser(int userId, string deviceType);
    Task<AccessTokenDto> RefreshUser(UserRefreshToken userRefreshToken);
    
    Task<UserRefreshToken?> GetUserRefreshToken(string refreshToken);

    Task<string> MakeAuthor(int id, MakeAuthorRequestDto request);
}