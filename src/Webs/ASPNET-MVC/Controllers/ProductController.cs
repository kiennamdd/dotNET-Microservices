using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ICatalogService _catalogService;

        public ProductController(ILogger<ProductController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        private IActionResult RedirectToProductDetails(string productId)
        {
            return RedirectToAction(nameof(ProductDetails), new {
                ProductId = productId
            });
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductIndex()
        {
            ResponseDto<IEnumerable<ProductDto>> response = await _catalogService.GetProductList();

            IEnumerable<ProductDto> list = response.Result ?? new List<ProductDto>();

            return View(list);
        }

        [HttpGet]
        [Route("/create")]
        [Authorize(Roles = Roles.ADMIN)]
        public IActionResult ProductCreate()
        {
            var model = new ProductCreateRequest(); 
            return View(model);
        }

        [HttpPost]
        [Route("/create")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductCreate([FromForm] ProductCreateRequest productCreateRequest)
        {
            if(!ModelState.IsValid)
            {
                return View(productCreateRequest);
            }

            ResponseDto<ProductCreateResponse> response = await _catalogService.CreateProduct(productCreateRequest);

            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return View(productCreateRequest);
            }
            else
            {
                TempData["Success"] = "Product created successfully!";
            }

            if(response.Result is null)
            {
                return RedirectToAction(nameof(ProductIndex));
            }

            return RedirectToProductDetails(response.Result.ProductId.ToString() ?? "");
        }

        [HttpGet]
        [Route("{productId}")]
        public async Task<IActionResult> ProductDetails(string productId)
        {
            if(!Guid.TryParse(productId, out _))
            {
                return NotFound();
            }

            ResponseDto<ProductDto> response = await _catalogService.GetProductDetails(productId);
            if(response.IsSuccess == false || response.Result is null)
            {
                return NotFound();
            }

            ProductDto model = response.Result;
            return View(model);
        }

        [HttpPost]
        [Route("{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductDelete(string productId)
        {
            if(!Guid.TryParse(productId, out _))
            {
                return NotFound();
            }

            ResponseDto response = await _catalogService.DeleteProduct(productId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Product marked as deleted successfully!";
            }

            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpPost]
        [Route("permanent-delete/{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductPermanentDelete(string productId)
        {
            if(!Guid.TryParse(productId, out _))
            {
                return NotFound();
            }

            ResponseDto response = await _catalogService.PermanentDeleteProduct(productId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Product permanent deleted successfully!";
            }

            return RedirectToAction(nameof(ProductIndex));
        }

        [HttpGet]
        [Route("update/{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductUpdate(string productId)
        {
            if(!Guid.TryParse(productId, out _))
            {
                return NotFound();
            }

            ResponseDto<ProductDto> response = await _catalogService.GetProductDetails(productId);
            if(response.IsSuccess == false || response.Result is null)
            {
                ViewData["Error"] = response.Message;
                return NotFound();
            }

            ProductDto p = response.Result;

            var updateRequest = new ProductUpdateRequest
            {
                Id = p.Id.ToString(),
                AppliedCouponCode = p.AppliedCouponCode,
                BrandName = p.BrandName,
                CategoryName = p.CategoryName,
                Description = p.Description,
                Name = p.Name,
                Origin = p.Origin,
                Price = p.Price
            };

            return View(updateRequest);
        }

        [HttpPost]
        [Route("update/{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductUpdate(string productId, [FromForm] ProductUpdateRequest productUpdateRequest)
        {
            if(!ModelState.IsValid)
            {
                return View(productUpdateRequest);
            }

            ResponseDto response = await _catalogService.UpdateProduct(productUpdateRequest);

            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Success"] = "Product updated successfully!";
            }

            return RedirectToProductDetails(productUpdateRequest.Id);
        }

        [HttpPost]
        [Route("add-images/{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductAddImages(string productId, [FromForm] ProductImagesAddRequest productImagesAddRequest)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToProductDetails(productImagesAddRequest.ProductId);
            }

            ResponseDto response = await _catalogService.AddProductImages(productImagesAddRequest);

            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToProductDetails(productImagesAddRequest.ProductId);
            }
            else
            {
                TempData["Success"] = "Product images added successfully!";
            }

            return RedirectToProductDetails(productImagesAddRequest.ProductId);
        }

        [HttpPost]
        [Route("remove-images/{productId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> ProductDeleteImages(string productId, [FromForm] ProductImagesDeleteRequest productImagesDeleteRequest)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToProductDetails(productImagesDeleteRequest.ProductId);
            }

            ResponseDto response = await _catalogService.DeleteProductImages(productImagesDeleteRequest);

            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return RedirectToProductDetails(productImagesDeleteRequest.ProductId);
            }
            else
            {
                TempData["Success"] = "Product images removed successfully!";
            }

            return RedirectToProductDetails(productImagesDeleteRequest.ProductId);
        }
    }
}