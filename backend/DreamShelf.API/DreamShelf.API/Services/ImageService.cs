using DreamShelf.API.Common.Interfaces;
using DreamShelf.API.Entities;
using DreamShelf.API.Enums;
using DreamShelf.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DreamShelf.API.Services;

public class ImageService: IImageService
{
    private readonly string _imageFolderPath;
    private readonly ApplicationDbContext _context;
    public ImageService(ApplicationDbContext context)
    {
        _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        
        if (!Directory.Exists(_imageFolderPath))
        {
            Directory.CreateDirectory(_imageFolderPath);
        }

        _context = context;
    }
    
    public async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        var filePath = Path.Combine(_imageFolderPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"{fileName}";
    }
    
    public async Task<string> SaveImageAsync(byte[] byteArray, string extension)
    {
        var fileName = $"{Guid.NewGuid()}.{extension}";
        var filePath = Path.Combine(_imageFolderPath, fileName);
        
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(byteArray);
        

        return $"{fileName}";
    }
    
    public async Task SaveImagesToDbAsync(List<string> images, int bookId, Guid? chapterId = null)
    {
        foreach (var image in images)
        {
            await _context.BookImages.AddAsync(new BookImage
            {
                ImageId = image,
                BookId = bookId,
                ChapterId =  chapterId
            });
        }
    }

    public async Task DeleteImageAsync(string fileName, int authorId, Role role)
    {
        int? bookId = await _context.BookImages
            .Where(x => x.ImageId == fileName)
            .Select(y => y.BookId)
            .FirstOrDefaultAsync();
        
        bool isImageAssigned = bookId != 0;
        
        if (isImageAssigned)
        {
            var isAuthor = await _context.Books
                .Where(b => b.Id == bookId && b.Authors.Any(a => a.Id == authorId || (
                    role == Role.Admin && b.AddedBy == Role.Admin)))
                .AnyAsync();
            
            if(!isAuthor)
                return;

            try
            {
                await _context.BookImages
                    .Where(bi => bi.ImageId == fileName)
                    .ExecuteDeleteAsync();
            }
            catch
            {
                return;
            }
        }
        
        var filePath = Path.Combine(_imageFolderPath, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public void DeleteImages(List<string> images)
    {
        foreach (var image in images)
        {
            string path = Path.Combine(_imageFolderPath, image);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}