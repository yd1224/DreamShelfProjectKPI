using System.ComponentModel.DataAnnotations;
using DreamShelf.API.Enums;

namespace DreamShelf.API.Entities;

public class User
{
    public int Id { get; set; }
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = [];
}