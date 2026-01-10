using DreamShelf.API.Enums;

namespace DreamShelf.API.Common.Interfaces;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile imageFile);
    Task<string> SaveImageAsync(byte[] byteArray, string extension);
    Task SaveImagesToDbAsync(List<string> images, int bookId, Guid? chapterId = null);
    Task DeleteImageAsync(string fileName, int authorId, Role role);
    void DeleteImages(List<string> images);
}