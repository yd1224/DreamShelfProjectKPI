using DreamShelf.API.Common.Dtos.Service;
using Mammoth;
using System.Text.RegularExpressions;
using DreamShelf.API.Common.Interfaces;

namespace DreamShelf.API.Services;

public partial class DocumentParserService (IImageService imageService) : IDocumentParserService
{
    private readonly IImageService  _imageService = imageService;

    public async Task<List<ChapterServiceDto>> ParseAndSplitIntoChapters(IFormFile document,  string splitBy, int bookId)
    {
        await using var stream = document.OpenReadStream();

        // 1. Convert entire DOCX to HTML
        string fullHtml = ConvertToHtmlWithMammoth(stream, bookId);
        var (headingRegex, headingSplitRegex) = CreateHeadingRegexes(splitBy);

        // 2. Split by <h1> tags (case-insensitive)
        var chaptersHtml = headingSplitRegex.Split(fullHtml);

        List<ChapterServiceDto> chapters = [];

        foreach (var chapterHtml in chaptersHtml)
        {
            if (string.IsNullOrWhiteSpace(chapterHtml))
                continue;

            var newChapter = new ChapterServiceDto();

            // Extract title from <h1>
            var match = headingRegex.Match(chapterHtml);
            
            string title = match.Success ? match.Groups[1].Value.Trim() : "Untitled";
            title = AnchorRegex().Replace(title, "").Trim();

            // Remove the <h1> from content
            string content = match.Success ? chapterHtml.Substring(match.Index + match.Length).Trim() : chapterHtml.Trim();

            newChapter.Title = title;
            newChapter.Content = content;
            
            chapters.Add(newChapter);
        }

        return chapters;
    }

    private string ConvertToHtmlWithMammoth(Stream docxStream, int bookId)
    {
        var converter = new DocumentConverter();
        var result = converter.ConvertToHtml(docxStream);
        string html = result.Value;

        // convert <p> to <div>
        html = ParagraphRegex().Replace(html, "<div")
            .Replace("</p>", "</div>");

        return html;
    }

    public async Task<string> SaveChapterImages(string html, Guid chapterId, int bookId)
    {
        var matches = ImageRegex().Matches(html);
        List<string> images = [];
        string newHtml = html;
        
        foreach (Match match in matches.Cast<Match>().Reverse()) // Reverse to not break indexes
        {
            string type = match.Groups["type"].Value;
            string base64Data = match.Groups["data"].Value;
            byte[] imageBytes = Convert.FromBase64String(base64Data);
        
            string fileName = await _imageService.SaveImageAsync(imageBytes, type);
            images.Add(fileName);
            
            // Replace the Base64 src with the saved file path
            string newImgTag = $"<img src=\"[PATH]/{fileName}\" />";
            newHtml = newHtml.Remove(match.Index, match.Length)
                .Insert(match.Index, newImgTag);
        }
        
        await _imageService.SaveImagesToDbAsync(images, bookId, chapterId);

        return newHtml;
    }
    
    private static (Regex headingRegex, Regex headingSplitRegex) CreateHeadingRegexes(string splitBy)
    {
        string tag = splitBy.ToLower();

        // Regex to extract heading content
        var headingRegex = new Regex(
            $@"<{tag}\b[^>]*>(.*?)</{tag}>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
        );

        // Regex to split by heading
        var headingSplitRegex = new Regex(
            $@"(?i)(?=<{tag}\b)",
            RegexOptions.Compiled
        );

        return (headingRegex, headingSplitRegex);
    }

    [GeneratedRegex(@"<img\s+[^>]*src=[""']data:image/(?<type>.+?);base64,(?<data>.+?)[""'][^>]*>", RegexOptions.IgnoreCase, "en-UA")]
    private static partial Regex ImageRegex();
    [GeneratedRegex(@"<p\b")]
    private static partial Regex ParagraphRegex();
    [GeneratedRegex(@"<a[^>]*></a>", RegexOptions.IgnoreCase, "en-UA")]
    private static partial Regex AnchorRegex();
}
