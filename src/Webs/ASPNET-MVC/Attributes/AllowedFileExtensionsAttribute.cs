using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ASPNET_MVC.Enums;

namespace ASPNET_MVC.Attributes
{
    public class AllowedFileExtensionsAttribute: ValidationAttribute
    {
        private readonly FileType _validFileType;

        public AllowedFileExtensionsAttribute(FileType validFileType)
        {
            _validFileType = validFileType;    
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if(file != null)
            {
                bool isValid = true;
                string extension =  Path.GetExtension(file.FileName).ToLower();

                switch(_validFileType)
                {
                    case FileType.IMAGE:
                        var regex = new Regex(@"(\.(?i)(jpe?g|png|gif|bmp))$");
                        isValid = regex.IsMatch(extension);
                        break;
                }

                if(isValid == false)
                    return new ValidationResult($"Invalid file extensions!");
            }

            return ValidationResult.Success;
        }
    }
}