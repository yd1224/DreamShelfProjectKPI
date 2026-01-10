using System.ComponentModel.DataAnnotations;

namespace DreamShelf.API.Common.Dtos.Request;

public class UserAuthRequestDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [MinLength(6), MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}