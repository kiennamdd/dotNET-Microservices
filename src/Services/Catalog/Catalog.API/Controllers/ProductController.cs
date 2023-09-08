using AutoMapper;
using Catalog.API.Domain.Constants;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Catalog.API.Interfaces.Infrastructure;
using Catalog.API.Models;
using EventBus.Events;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;
        private readonly IFileService _fileService;
        private readonly IValidator<ProductCreateRequest> _productCreateValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateValidator;
        private readonly IValidator<ProductImagesAddRequest> _productImagesAddValidator;
        private readonly IValidator<ProductImagesDeleteRequest> _productImagesDeleteValidator;
        private readonly IPublishEndpoint _publishEndpoint;

        public ProductController(IMapper mapper,
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IBrandRepository brandRepository,
            IUnitOfWork unitOfWork, 
            IProductImageRepository productImageRepository,
            IDiscountService discountService,
            IFileService fileService,
            IValidator<ProductCreateRequest> productCreateValidator,
            IValidator<ProductUpdateRequest> productUpdateValidator,
            IValidator<ProductImagesAddRequest> productImagesAddValidator,
            IValidator<ProductImagesDeleteRequest> productImagesDeleteValidator,
            IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;

            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _productImageRepository = productImageRepository;

            _discountService = discountService;
            _fileService = fileService;

            _productCreateValidator = productCreateValidator;
            _productUpdateValidator = productUpdateValidator;
            _productImagesAddValidator = productImagesAddValidator;
            _productImagesDeleteValidator = productImagesDeleteValidator;

            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ResponseDto> Get(string? containName, 
                string? category, 
                string? brand, 
                string? origin, 
                string? orderBy,
                double? min = 0, 
                double? max = 0,
                bool showDeleted = true,
                int page = 1)
        {
            var list = await _productRepository.GetListAsync(o => showDeleted || o.IsDeleted == false, includeProperties: "Category,Brand");

            if(!string.IsNullOrEmpty(containName))
            {
                list = list.Where(o => o.Name.Contains(containName, StringComparison.InvariantCultureIgnoreCase));
            }

            if(!string.IsNullOrEmpty(category))
            {
                list = list.Where(o => o.Category != null && o.Category.Name.Equals(category, StringComparison.InvariantCultureIgnoreCase));
            }

            if(!string.IsNullOrEmpty(brand))
            {
                list = list.Where(o => o.Brand != null && o.Brand.Name.Equals(brand, StringComparison.InvariantCultureIgnoreCase));
            }

            if(!string.IsNullOrEmpty(origin))
            {
                list = list.Where(o => o.Origin.Equals(origin, StringComparison.InvariantCultureIgnoreCase));
            }

            if(min > 0)
            {
                list = list.Where(o => o.Price >= min);
            }

            if(max > 0)
            {
                list = list.Where(o => o.Price <= max);
            }

            switch(orderBy)
            {
                case "name_asc":
                    list = list.OrderBy(o => o.Name);
                    break;
                case "name_desc":
                    list = list.OrderByDescending(o => o.Name);
                    break;
                case "price_asc":
                    list = list.OrderBy(o => o.Price);
                    break;
                case "price_desc":
                    list = list.OrderByDescending(o => o.Price);
                    break;
                default:
                    list = list.OrderBy(o => o.CreatedAt);
                    break;
            }

            int pageSize = 20;
            var pagedList = list.ToPagedList(page, pageSize).ToList();

            var listDto = _mapper.Map<IEnumerable<ProductDto>>(list);

            return ResponseDto.Success(result: listDto);
        }

        [HttpGet]
        [Route("details/{id}")]
        public async Task<ResponseDto> Get(string id)
        {
            if (!Guid.TryParse(id, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier.");
            }

            var list = await _productRepository.GetListAsync(o => o.Id.Equals(productId), includeProperties: "Category,Brand,ProductImages");
            Product? product = list.FirstOrDefault();          

            if(product is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            return ResponseDto.Success(result: _mapper.Map<ProductDto>(product));
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> Post([FromForm] ProductCreateRequest productCreateRequest)
        {
            await _productCreateValidator.ValidateAndThrowAsync(productCreateRequest);

            Product product = _mapper.Map<Product>(productCreateRequest);

            Category? category = await _categoryRepository.GetByNameAsync(productCreateRequest.CategoryName.Trim());
            if(category is null)
            {
                category = new Category { Name = productCreateRequest.Name.Trim() };
                _categoryRepository.Add(category);
                await _unitOfWork.SaveChangesAsync();
            }
            product.CategoryId = category.Id;

            Brand? brand = await _brandRepository.GetByNameAsync(productCreateRequest.BrandName.Trim());
            if(brand is null)
            {
                brand = new Brand { Name = productCreateRequest.BrandName.Trim() };
                _brandRepository.Add(brand);
                await _unitOfWork.SaveChangesAsync();
            }
            product.BrandId = brand.Id;

            string couponCode = productCreateRequest.AppliedCouponCode.Trim();
            if(!string.IsNullOrEmpty(couponCode))
            {
                CouponDto? couponDto = await _discountService.GetCouponByCodeAsync(couponCode);
                
                if(couponDto != null)
                {
                    product.AppliedCouponCode = couponCode;
                    product.DiscountAmount = couponDto.DiscountAmount;
                    product.DiscountPercent = couponDto.DiscountPercent;
                }
            }

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            string saveFolder = StaticFolders.GetStaticFolderPath(StaticFolders.ProductImages);
            
            if(productCreateRequest.Thumbnail != null)
            {
                _fileService.SaveFile(productCreateRequest.Thumbnail, out string fileName, out string localPath, saveFolder);
                product.ThumbnailUrl = Path.Combine(baseUrl, StaticFolders.ProductImages, fileName);
                product.ThumbnailLocalPath = localPath;
            }

            var productImages = new List<ProductImage>();
            if(productCreateRequest.Images != null)
            {
                foreach(var image in productCreateRequest.Images)
                {
                    bool success = _fileService.SaveFile(image, out string fileName, out string localPath, saveFolder);
                    
                    var productImage = new ProductImage()
                    {
                        ImageName = "image",
                        ImageUrl = Path.Combine(baseUrl, StaticFolders.ProductImages, fileName),
                        ImageLocalPath = localPath
                    };

                    productImages.Add(productImage);
                }
            }

            product.ProductImages = productImages;

            _productRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success("Product created successfully!", product.Id);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> Delete(string id)
        {
            if (!Guid.TryParse(id, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier.");
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if(product is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            product.IsDeleted = true;
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            //  If product is changed to status "deleted" successfully, publish integration event
            await _publishEndpoint.Publish(new ProductDeletedEvent{
                ProductId = productId
            });

            return ResponseDto.Success("Product deleted successfully.");
        }

        [HttpDelete]
        [Route("permanentdelete/{id}")]
        public async Task<ResponseDto> PermanentDelete(string id)
        {
            if (!Guid.TryParse(id, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier.");
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if(product is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            if(product.IsDeleted == false)
            {
                return ResponseDto.Fail("Product must be mark as deleted before permanently deleting.");
            }

            if(!string.IsNullOrEmpty(product.ThumbnailLocalPath))
            {
                _fileService.DeleteFile(product.ThumbnailLocalPath);
            }

            var productImages = await _productImageRepository.GetListAsync(o => o.ProductId.Equals(product.Id));
            foreach(var productImage in productImages)
            {
                _fileService.DeleteFile(productImage.ImageLocalPath);
                _productImageRepository.Delete(productImage);
            }

            _productRepository.Delete(product);

            await _unitOfWork.SaveChangesAsync();

            return ResponseDto.Success("Product permanently deleted successfully.");
        }

        [HttpPut]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> Put(ProductUpdateRequest productUpdateRequest)
        {
            if(!Guid.TryParse(productUpdateRequest.Id, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier");
            }

            await _productUpdateValidator.ValidateAndThrowAsync(productUpdateRequest);

            var list = await _productRepository.GetListAsync(o => o.Id.Equals(productId), includeProperties: "Category,Brand");
            Product? product = list.FirstOrDefault();

            if(product is null)
            {
                return ResponseDto.Fail("Not found.");
            }
            
            string newCouponCode = productUpdateRequest.AppliedCouponCode.Trim();
            ProductCouponCodeChangedEvent? productCouponCodeChangeEvent = null;

            if(!product.AppliedCouponCode.Equals(newCouponCode, StringComparison.InvariantCultureIgnoreCase))
            {
                if(string.IsNullOrEmpty(newCouponCode))
                {
                    product.AppliedCouponCode = string.Empty;
                    product.DiscountAmount = 0;
                    product.DiscountPercent = 0;

                    productCouponCodeChangeEvent = new ProductCouponCodeChangedEvent{
                        ProductId = product.Id,
                        AppliedCouponCode = string.Empty
                    };
                }
                else
                {
                    CouponDto? couponDto = await _discountService.GetCouponByCodeAsync(newCouponCode);
                    if(couponDto != null)
                    {
                        product.AppliedCouponCode = newCouponCode;
                        product.DiscountAmount = couponDto.DiscountAmount;
                        product.DiscountPercent = couponDto.DiscountPercent;

                        productCouponCodeChangeEvent = new ProductCouponCodeChangedEvent{
                            ProductId = product.Id,
                            Price = product.Price,
                            AppliedCouponCode = product.AppliedCouponCode,
                            DiscountAmount = product.DiscountAmount,
                            DiscountPercent = product.DiscountPercent
                        };
                    }
                }
            }

            string brandNameUpdate = productUpdateRequest.BrandName.Trim();
            bool isSameBrand = brandNameUpdate.Equals(product.Brand?.Name, StringComparison.InvariantCultureIgnoreCase);
            if(!string.IsNullOrEmpty(brandNameUpdate) && !isSameBrand)
            {
                Brand? brand = await _brandRepository.GetByNameAsync(brandNameUpdate);
                if(brand is null)
                {
                    brand = new Brand { Name = brandNameUpdate };
                    _brandRepository.Add(brand);
                    await _unitOfWork.SaveChangesAsync();
                }

                product.BrandId = brand.Id;
            }

            string categoryNameUpdate = productUpdateRequest.CategoryName.Trim();
            bool isSameCategory = categoryNameUpdate.Equals(product.Category?.Name, StringComparison.InvariantCultureIgnoreCase);
            if(!string.IsNullOrEmpty(categoryNameUpdate) && !isSameCategory)
            {
                Category? category = await _categoryRepository.GetByNameAsync(categoryNameUpdate);
                if(category is null)
                {
                    category = new Category { Name = categoryNameUpdate };
                    _categoryRepository.Add(category);
                    await _unitOfWork.SaveChangesAsync();
                }
                product.CategoryId = category.Id;
            }

            bool isPriceChanged = product.Price != productUpdateRequest.Price;

            product.Name = productUpdateRequest.Name;
            product.Price = productUpdateRequest.Price;
            product.Description = productUpdateRequest.Description;
            product.Origin = productUpdateRequest.Origin;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // If product's price is changed, publish integration event
            if(isPriceChanged)
            {
                await _publishEndpoint.Publish(_mapper.Map<ProductPriceChangedEvent>(product));
            }
            // If product's applied coupon has been changed, publish integration event
            else if(productCouponCodeChangeEvent != null)
            {
                await _publishEndpoint.Publish(productCouponCodeChangeEvent);
            }

            return ResponseDto.Success("Product updated successfully!");
        }

        [HttpPost]
        [Route("RemoveImages")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> RemoveImages(ProductImagesDeleteRequest productImagesDeleteRequest)
        {
            if(!Guid.TryParse(productImagesDeleteRequest.ProductId, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier");
            }

            await _productImagesDeleteValidator.ValidateAndThrowAsync(productImagesDeleteRequest);

            var list = await _productRepository.GetListAsync(o => o.Id.Equals(productId), includeProperties: "ProductImages");
            Product? product = list.FirstOrDefault();
            if(product is null)
            {
                return ResponseDto.Fail("Invalid product identifier");
            }


            int count = 0;
            if(product.ProductImages != null && productImagesDeleteRequest.ProductImageIds != null)
            {
                List<Guid> deleteIds = productImagesDeleteRequest.ProductImageIds
                                                                    .Select(id => Guid.TryParse(id, out Guid guid) ? guid : Guid.Empty)
                                                                    .Where(guid => guid != Guid.Empty)
                                                                    .ToList();

                var matchedProductImages = product.ProductImages.Where(o => deleteIds.Contains(o.Id));

                foreach(var productImage in matchedProductImages)
                {
                    if(!string.IsNullOrEmpty(productImage.ImageLocalPath))
                    {
                        _fileService.DeleteFile(productImage.ImageLocalPath);
                    }

                    _productImageRepository.Delete(productImage);
                }

                count = await _unitOfWork.SaveChangesAsync();
            }

            return ResponseDto.Success($"Product images deleted successfully. Count: {count}");
        }

        [HttpPost]
        [Route("AddImages")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> AddImages([FromForm] ProductImagesAddRequest productImagesAddRequest)
        {
            if(!Guid.TryParse(productImagesAddRequest.ProductId, out Guid productId))
            {
                return ResponseDto.Fail("Invalid product identifier");
            }

            await _productImagesAddValidator.ValidateAndThrowAsync(productImagesAddRequest);

            var list = await _productRepository.GetListAsync(o => o.Id.Equals(productId));
            Product? product = list.FirstOrDefault();
            if(product is null)
            {
                return ResponseDto.Fail("Invalid product identifier");
            }

            int count = 0;
            if(productImagesAddRequest.Images != null)
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                foreach(var image in productImagesAddRequest.Images)
                {
                    string saveFolder = StaticFolders.GetStaticFolderPath(StaticFolders.ProductImages);
                    _fileService.SaveFile(image, out string fileName, out string localPath, saveFolder);

                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        ImageName = "image",
                        ImageUrl = Path.Combine(baseUrl, StaticFolders.ProductImages, fileName),
                        ImageLocalPath = localPath
                    };

                    _productImageRepository.Add(productImage);
                }

                count = await _unitOfWork.SaveChangesAsync();
            }

            return ResponseDto.Success($"Product images added successfully. Count: {count}");
        }
    }
}