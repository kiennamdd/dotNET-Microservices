using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Identity;
using Cart.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class IdentityController : Controller
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IIdentityService _identityService;
        private readonly ITokenProvider _tokenProvider;

        public IdentityController(ILogger<IdentityController> logger, IIdentityService identityService, ITokenProvider tokenProvider)
        {
            _logger = logger;
            _identityService = identityService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        [Route("sign-in")]
        public IActionResult SignIn([FromQuery] string? returnUrl)
        {
            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            // todo: add returnUrl to sign in request

            var signInRequest = new SignInRequest();
            return View(signInRequest);
        }

        [HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> SignIn([FromForm] SignInRequest signInRequest)
        {
            if(!ModelState.IsValid)
                return View(signInRequest);

            ResponseDto<SignInResponse> response = await _identityService.SignIn(signInRequest);
            if(response.IsSuccess == false || response.Result is null)
            {
                TempData["Error"] = !string.IsNullOrEmpty(response.Message) ? response.Message : "Sign in fail!";
                return View(signInRequest);
            }
            else
            {
                TempData["Success"] = "Signed in successfully!";
            }

            await SignInUser(response.Result.AccessToken);
            _tokenProvider.SetAccessToken(response.Result.AccessToken);

            // todo: Redirect to return url            
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            var registerRequest = new RegisterRequest();

            ViewBag.RoleList = new List<SelectListItem>
            {
                new (Roles.ADMIN, Roles.ADMIN),
                new ("User", "User"),
            };

            return View(registerRequest);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest registerRequest)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.RoleList = new List<SelectListItem>
                {
                    new (Roles.ADMIN, Roles.ADMIN),
                    new ("User", "User"),
                };
                return View(registerRequest);
            }

            ResponseDto response = await _identityService.Register(registerRequest);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
                return View(registerRequest);
            }
            else
            {
                TempData["Success"] = "Registered successfully!";
            }

            // todo: Redirect to return url            
            return RedirectToAction(nameof(SignIn));
        }

        [HttpPost]
        [Route("sign-out")]
        public async Task<IActionResult> SignOut2()
        {
            await SignOutUser();
            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(HomeController.Index), "/");
        }

        private async Task SignInUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaims(jwt.Claims);

            var roleList = jwt.Claims.Where(u => u.Type == "role");
            foreach(var role in roleList)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Value));
            }

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private async Task SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        //todo: Assign role, Delete user, View user list
    }
}