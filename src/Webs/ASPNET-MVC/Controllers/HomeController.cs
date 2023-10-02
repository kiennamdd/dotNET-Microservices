using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Catalog;
using Cart.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASPNET_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICatalogService _catalogService;

        public HomeController(ILogger<HomeController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            ResponseDto<IEnumerable<ProductDto>> response = await _catalogService.GetProductList();

            IEnumerable<ProductDto> list = response.Result ?? new List<ProductDto>();

            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}