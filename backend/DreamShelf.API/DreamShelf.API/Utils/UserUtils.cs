namespace DreamShelf.API.Utils;

public static class UserUtils
{
    public static string DisplayName(string? firstName, string? lastName, string? pseudonym)
    {
        if (firstName != null && lastName !=null)
        {
            return pseudonym != null ? $"{firstName} {lastName} ({pseudonym})" : $"{firstName} {lastName}";
        }
        else if (pseudonym !=null)
        {
            return pseudonym;
        }
        else
        {
            return "";
        }
        
    }
    
    public static string DisplayName(string? fullname, string? pseudonym)
    {
        if (!string.IsNullOrEmpty(fullname))
        {
            return !string.IsNullOrEmpty(pseudonym) ? $"{fullname} ({pseudonym})" : $"{fullname}";
        }
        else if (!string.IsNullOrEmpty(pseudonym))
        {
            return pseudonym;
        }
        else
        {
            return "";
        }
        
    }
}