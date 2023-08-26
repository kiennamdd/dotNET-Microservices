
using Catalog.API.Domain.Enums;
using Catalog.API.Interfaces;
using Catalog.API.Models;
using FluentValidation;

namespace Catalog.API.Validators
{
    public class ProductCreateRequestValidator: AbstractValidator<ProductCreateRequest>
    {
        private readonly IFileService _fileService;

        public ProductCreateRequestValidator(IFileService fileService)
        {
            _fileService = fileService;

            RuleFor(o => o.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(o => o.Description)
                .MaximumLength(500);

            RuleFor(o => o.Origin)
                .MaximumLength(10);

            RuleFor(o => o.Price)
                .Must(price => price >= 0)
                .WithMessage("[Price] can not have negative value.");

            RuleFor(o => o.CategoryName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(o => o.BrandName)
                .NotEmpty()
                .MaximumLength(50);

            int maxNumberOfImagesOnce = 40;
            RuleFor(o => o.Images)
                .Must(images => (images is null) || (images != null && images.Count() <= maxNumberOfImagesOnce))
                .WithMessage($"Can not upload more than {maxNumberOfImagesOnce} file at once time.");

            int maxSizeMB = 5;
            RuleFor(o => o.Thumbnail)
                .NotNull()
                .Must(o => o != null && _fileService.IsValidFileSize(o, maxSizeMB))
                .WithMessage($"Thumbnail size should not exceed {maxSizeMB} MB")
                .Must(o => o != null && _fileService.IsValidFileExtension(o, FileTypes.IMAGE))
                .WithMessage("Thumbnail extension is not valid, valid image extension: png, jpg, jpeg.");

            RuleForEach(o => o.Images)
                .Must(o => _fileService.IsValidFileSize(o, maxSizeMB))
                .WithMessage($"File size should not exceed {maxSizeMB} MB")
                .Must(o => _fileService.IsValidFileExtension(o, FileTypes.IMAGE))
                .WithMessage("File extension is not valid, valid image extension: png, jpg, jpeg.");
        }
    }
}