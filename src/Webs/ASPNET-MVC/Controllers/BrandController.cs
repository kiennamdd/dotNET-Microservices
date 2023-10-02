using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class BrandController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ICatalogService _catalogService;

        public BrandController(ILogger<ProductController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> BrandIndex()
        {
            ResponseDto<IEnumerable<BrandDto>> response = await _catalogService.GetBrandList();

            IEnumerable<BrandDto> list = response.Result ?? new List<BrandDto>();

            return View(list);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public IActionResult BrandCreate()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> BrandCreate(string brandName)
        {
            if(string.IsNullOrWhiteSpace(brandName))
            {
                return View();
            }

            ResponseDto response = await _catalogService.CreateBrand(brandName);
            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return View();
            }
            else
            {
                TempData["Success"] = "Brand created successfully!";
            }

            return RedirectToAction(nameof(BrandIndex));
        }
    }
}