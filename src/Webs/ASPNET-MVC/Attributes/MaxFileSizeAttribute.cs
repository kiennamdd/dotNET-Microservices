using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Attributes
{
    public class MaxFileSizeAttribute: ValidationAttribute
    {
        private readonly int _maxSizeMB;

        public MaxFileSizeAttribute(int maxSizeMB)
        {
            _maxSizeMB = maxSizeMB;    
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if(file != null)
            {
                bool isValid = (file.Length/(1024*1024)) <= _maxSizeMB;

                if(isValid == false)
                    return new ValidationResult($"Invalid file extensions!");
            }

            return ValidationResult.Success;
        }
    }
}