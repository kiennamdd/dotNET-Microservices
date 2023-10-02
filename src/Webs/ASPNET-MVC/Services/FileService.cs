

using System.Text.RegularExpressions;
using ASPNET_MVC.Enums;
using ASPNET_MVC.Interfaces;

namespace ASPNET_MVC.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;    
        }

        public bool IsValidFileExtension(IFormFile file, FileType validFileType)
        {
            bool isValid = true;
            string extension =  Path.GetExtension(file.FileName).ToLower();

            if(string.IsNullOrEmpty(extension))
                return false;

            switch(validFileType)
            {
                case FileType.IMAGE:
                    var regex = new Regex(@"(\.(?i)(jpe?g|png|gif|bmp))$");
                    isValid = regex.IsMatch(extension);
                    break;
            }

            return isValid;
        }

        public bool IsValidFileSize(IFormFile file, int maxSizeMB)
        {
            bool isValid = (file.Length/(1024*1024)) <= maxSizeMB;

            return isValid;
        }
   }
}