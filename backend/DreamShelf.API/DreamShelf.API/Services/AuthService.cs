using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DreamShelf.API.Common.Dtos.Response;
using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Configurations.Options;
using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DreamShelf.API.Services;

public class AuthService (
    IOptions<JwtOptions> jwtOptions,
    IOptions<AuthorizationOptions> authOptions,
    ApplicationDbContext context): IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly AuthorizationOptions _authOptions = authOptions.Value;
    private readonly ApplicationDbContext _context = context;
    
    public string GeneratePasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password + _authOptions.Secret, workFactor: 12);
    }
    
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password + _authOptions.Secret, passwordHash);
    }
    
    public async Task<string> GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email),
            new Claim("role", user.Role.ToString())
        };

        if (user.Role is Role.Author or Role.Admin)
        {
            var authorId = await _context.Authors
                .Where(a => a.UserId == user.Id)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (authorId > 0)
            {
                claims.Add(new Claim("authorId", authorId.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public (string token, DateTime expirationDate)  GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return (Convert.ToBase64String(randomNumber),  DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays));
    }
}