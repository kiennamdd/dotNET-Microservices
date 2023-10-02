
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using ASPNET_MVC.Enums;

namespace ASPNET_MVC.Attributes
{
    public class AllowedFileExtensionsListAttribute: ValidationAttribute
    {
        private readonly FileType _validFileType;

        public AllowedFileExtensionsListAttribute(FileType validFileType)
        {
            _validFileType = validFileType;    
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IEnumerable<IFormFile> list)
            {
                var regex = _validFileType switch 
                {
                    FileType.IMAGE => new Regex(@"(\.(?i)(jpe?g|png|gif|bmp))$"),
                    _ => null
                };

                if(regex is null)
                    return ValidationResult.Success;

                foreach (IFormFile file in list)
                {
                    string extension =  Path.GetExtension(file.FileName).ToLower();
                    bool isValid = regex.IsMatch(extension);

                    if(isValid == false)
                        return new ValidationResult($"Invalid file extensions!");
                }
            }

            return ValidationResult.Success;
        }
    }
}