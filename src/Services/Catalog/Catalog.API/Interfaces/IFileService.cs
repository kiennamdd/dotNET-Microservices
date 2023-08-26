
using Catalog.API.Domain.Enums;

namespace Catalog.API.Interfaces
{
    public interface IFileService
    {
        bool IsValidFileExtension(IFormFile file, FileTypes validFileType);
        bool IsValidFileSize(IFormFile file, int maxSize);

        bool SaveFile(IFormFile file, out string savedFileName, out string savedLocalPath, string saveToFolder = "");
        bool HasFile(string localPath);
        bool DeleteFile(string localPath);
    }
}