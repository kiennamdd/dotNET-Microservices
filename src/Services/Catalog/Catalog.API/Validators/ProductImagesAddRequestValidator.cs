
using Catalog.API.Domain.Enums;
using Catalog.API.Interfaces;
using Catalog.API.Models;
using FluentValidation;

namespace Catalog.API.Validators
{
    public class ProductImagesAddRequestValidator: AbstractValidator<ProductImagesAddRequest>
    {
        private readonly IFileService _fileService;

        public ProductImagesAddRequestValidator(IFileService fileService)
        {
            _fileService = fileService;
            
            RuleFor(o => o.ProductId)
                .NotEmpty();

            int maxNumberOfImagesOnce = 40;
            RuleFor(o => o.Images)
                .Must(images => images != null && images.Any())
                .WithMessage("[Images] list must contains at least one item.")
                .Must(images => (images is null) || (images != null && images.Count() <= maxNumberOfImagesOnce))
                .WithMessage($"Can not upload more than {maxNumberOfImagesOnce} file at once time.");

            int maxSizeMB = 5;
            RuleForEach(o => o.Images)
                .Must(o => _fileService.IsValidFileSize(o, maxSizeMB))
                .WithMessage($"File size should not exceed {maxSizeMB} MB")
                .Must(o => _fileService.IsValidFileExtension(o, FileTypes.IMAGE))
                .WithMessage("File extension is not valid, valid image extension: png, jpg, jpeg.");
        }
    }
}