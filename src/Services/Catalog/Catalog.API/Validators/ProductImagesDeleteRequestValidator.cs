
using Catalog.API.Models;
using FluentValidation;

namespace Catalog.API.Validators
{
    public class ProductImagesDeleteRequestValidator: AbstractValidator<ProductImagesDeleteRequest>
    {
        public ProductImagesDeleteRequestValidator()
        {
            RuleFor(o => o.ProductId)
                .NotEmpty();

            RuleFor(o => o.ProductImageIds)
                .Must(imageIds => imageIds != null && imageIds.Any())
                .WithMessage("[ProductImageIds] list must contains at least one item.");
        }
    }
}