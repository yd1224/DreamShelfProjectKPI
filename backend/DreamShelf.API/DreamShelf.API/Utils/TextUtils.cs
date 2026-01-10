using System.Text;

namespace DreamShelf.API.Utils;

public static class TextUtils
{
    public static string RemovePunctuation(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder sb = new StringBuilder();

        foreach (char c in input)
        {
            if (!char.IsPunctuation(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

}