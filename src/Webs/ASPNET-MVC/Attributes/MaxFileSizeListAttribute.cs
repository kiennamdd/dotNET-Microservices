using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Attributes
{
    public class MaxFileSizeListAttribute: ValidationAttribute
    {
        private readonly int _maxSizeMB;

        public MaxFileSizeListAttribute(int maxSizeMB)
        {
            _maxSizeMB = maxSizeMB;    
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IEnumerable<IFormFile> list)
            {
                foreach (IFormFile file in list)
                {
                    bool isValid = (file.Length / (1024 * 1024)) <= _maxSizeMB;

                    if (isValid == false)
                        return new ValidationResult($"Invalid file extensions!");
                }
            }

            return ValidationResult.Success;
        }
    }
}