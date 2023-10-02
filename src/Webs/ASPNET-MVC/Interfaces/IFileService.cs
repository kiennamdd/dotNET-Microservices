
using ASPNET_MVC.Enums;

namespace ASPNET_MVC.Interfaces
{
    public interface IFileService
    {
        bool IsValidFileExtension(IFormFile file, FileType validFileType);
        bool IsValidFileSize(IFormFile file, int maxSize);
    }
}