using DreamShelf.API.Entities;

namespace DreamShelf.API.Common.Interfaces;

public interface IAuthService
{
    string GeneratePasswordHash(string password);
    bool VerifyPassword(string password, string passwordHash);
    Task<string> GenerateJwtToken(User user);
    (string token, DateTime expirationDate)  GenerateRefreshToken();
}