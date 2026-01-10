using System.Text.Json.Serialization;

namespace DreamShelf.API.Common.Dtos.Response;

public class AuthorResponseDto
{
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string DisplayName { get; set; } = string.Empty;
}