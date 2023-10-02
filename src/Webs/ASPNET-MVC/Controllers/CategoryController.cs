using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class CategoryController : Controller
    {
       private readonly ILogger<ProductController> _logger;
        private readonly ICatalogService _catalogService;

        public CategoryController(ILogger<ProductController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CategoryIndex()
        {
            ResponseDto<IEnumerable<CategoryDto>> response = await _catalogService.GetCategoryList();

            IEnumerable<CategoryDto> list = response.Result ?? new List<CategoryDto>();

            return View(list);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CategoryCreate(string categoryName)
        {
            if(string.IsNullOrWhiteSpace(categoryName))
            {
                return View();
            }

            ResponseDto response = await _catalogService.CreateCategory(categoryName);
            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
                return View();
            }
            else
            {
                TempData["Success"] = "Category created successfully!";
            }

            return RedirectToAction(nameof(CategoryIndex));
        }
    }
}