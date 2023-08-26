using System.Text.RegularExpressions;
using Catalog.API.Domain.Enums;
using Catalog.API.Interfaces;

namespace Catalog.API.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;    
        }

        public bool DeleteFile(string localPath)
        {
            try
            {
                string fullLocalPath = Path.Combine(Directory.GetCurrentDirectory(), localPath);

                var file = new FileInfo(fullLocalPath);
                if(file.Exists)
                {
                    file.Delete();
                }
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"An error occurred while deleting file: {localPath}");
                return false;
            }
        }

        public bool HasFile(string localPath)
        {
            try
            {
                string fullLocalPath = Path.Combine(Directory.GetCurrentDirectory(), localPath);
                var file = new FileInfo(fullLocalPath);
                return file.Exists;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"An error occurred while finding file: {localPath}");
                return false;
            }
        }

        public bool IsValidFileExtension(IFormFile file, FileTypes validFileType)
        {
            bool isValid = true;
            string extension =  Path.GetExtension(file.FileName).ToLower();

            if(string.IsNullOrEmpty(extension))
                return false;

            switch(validFileType)
            {
                case FileTypes.IMAGE:
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

        public bool SaveFile(IFormFile file, out string savedFileName, out string savedLocalPath, string saveToFolder = "")
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string defaultPath = Path.GetFullPath(Path.Combine(@"wwwroot/SavedFiles", fileName));
                string chosenPath = Path.GetFullPath(Path.Combine(saveToFolder, fileName));

                string localPath = string.IsNullOrEmpty(saveToFolder) ? defaultPath : chosenPath;
                string fullLocalPath = Path.Combine(Directory.GetCurrentDirectory(), localPath);

                string? directory = Path.GetDirectoryName(fullLocalPath);
                if(!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var fileStream = new FileStream(fullLocalPath, FileMode.Create);

                file.CopyTo(fileStream);

                savedFileName = fileName;
                savedLocalPath = localPath;
                return true;
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"An error occurred while saving file: {file.FileName}");

                savedFileName = string.Empty;
                savedLocalPath = string.Empty;
                return false;
            }
        }
    }
}