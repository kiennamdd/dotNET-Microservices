using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Order;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly ICurrentUser _currentUser;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, ICurrentUser currentUser)
        {
            _logger = logger;
            _orderService = orderService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> OrderIndex([FromQuery] string? status)
        {
            var response = new ResponseDto<IEnumerable<CustomerOrderDto>>();

            if(_currentUser.IsInRole(Roles.ADMIN))
            {
                response = await _orderService.GetOrderList();
            }
            else
            {
                response = await _orderService.GetOrderHistory();
            }

            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }

            IEnumerable<CustomerOrderDto> list = response.Result ?? new List<CustomerOrderDto>();

            if(!string.IsNullOrEmpty(status) && !status.Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                list = list.Where(o => o.Status == status);
            }

            return View(list);
        }

        [HttpPost]
        [Route("cancel/{orderId}")]
        public async Task<IActionResult> OrderCancel(string orderId)
        {
            if(!Guid.TryParse(orderId, out _))
            {
                return NotFound();
            }

            ResponseDto response = await _orderService.CancelOrder(orderId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
                return BadRequest();
            }

            return RedirectToAction(nameof(OrderIndex));
        }
        
        [HttpPost]
        [Route("mark-as-shipped/{orderId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> OrderShipped(string orderId)
        {
            if(!Guid.TryParse(orderId, out _))
            {
                return NotFound();
            }

            ResponseDto response = await _orderService.SetShippedOrderStatus(orderId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
                return BadRequest();
            }

            return RedirectToAction(nameof(OrderIndex));
        }

        [HttpGet]
        [Route("{orderId}")]
        public async Task<IActionResult> OrderDetails(string orderId)
        {
            if(!Guid.TryParse(orderId, out _))
            {
                return NotFound();
            }

            ResponseDto<CustomerOrderDto> response = await _orderService.GetOrderDetails(orderId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
                return BadRequest();
            }

            CustomerOrderDto order = response.Result ?? new CustomerOrderDto();
            return View(order);
        }
    }
}