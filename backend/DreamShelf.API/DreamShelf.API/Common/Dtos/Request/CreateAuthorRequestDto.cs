namespace DreamShelf.API.Common.Dtos.Request;

public class CreateAuthorRequestDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Pseudonym { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
}