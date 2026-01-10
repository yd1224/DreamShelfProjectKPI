namespace DreamShelf.API.Configurations.Options;

public class AuthorizationOptions
{
    public const string SectionName = "Authorization";
    public string Secret { get; set; } = string.Empty;
}