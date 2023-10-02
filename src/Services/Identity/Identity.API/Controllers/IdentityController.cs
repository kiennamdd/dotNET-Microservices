using AutoMapper;
using EventBus.Events;
using FluentValidation;
using Identity.API.Domain.Entities;
using Identity.API.Interfaces;
using Identity.API.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<SignInRequest> _signInValidator;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public IdentityController(IIdentityService identityService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<SignInRequest> signInValidator,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _identityService = identityService;    

            _registerValidator = registerValidator;
            _signInValidator = signInValidator;

            _mapper = mapper;

            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ResponseDto> Register([FromBody] RegisterRequest registerRequest)
        {
            await _registerValidator.ValidateAndThrowAsync(registerRequest);

            var user = await _identityService.FindUserByEmailAsync(registerRequest.Email);
            if(user != null)
            {
                return ResponseDto.Fail("Email already exists.");
            }

            var newUser = new ApplicationUser
            {
                FullName = registerRequest.FullName,
                PhoneNumber = registerRequest.PhoneNumber,
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                EmailConfirmed = false
            };

            var succeed = await _identityService.CreateUserAsync(newUser, registerRequest.Password);
            if(!succeed)
            {
                return ResponseDto.Fail("Fail to register user.");
            }

            await _publishEndpoint.Publish(_mapper.Map<UserCreatedIntegrationEvent>(newUser));

            return ResponseDto.Success("User has been registered successfully.");
        }

        [HttpPost]
        [Route("SignIn")]
        public async Task<ResponseDto> SignIn([FromBody] SignInRequest signInRequest)
        {
            await _signInValidator.ValidateAndThrowAsync(signInRequest);

            var accessToken = await _identityService.SignInAsync(signInRequest.Email, signInRequest.Password);
            if(string.IsNullOrEmpty(accessToken))
            {
                return ResponseDto.Fail("Wrong email or password.");
            }

            var signInResponse = new SignInResponse
            {
                AccessToken = accessToken
            };

            return ResponseDto.Success("Signed in successfully.", result: signInResponse);
        }

        [HttpPost]
        [Route("AssignRole/{userId}")]
        public async Task<ResponseDto> AssignRole(string userId, [FromBody] string roleName)
        {
            if(!Guid.TryParse(userId, out Guid guid))
            {
                return ResponseDto.Fail("Invalid identifier.");
            }

            var succeed = await _identityService.AssignRoleAsync(guid, roleName);
            if(!succeed)
            {
                return ResponseDto.Fail($"Unable to assign role '{roleName}' to user.");
            }

            return ResponseDto.Success($"User has been assigned to role '{roleName}'.");
        }

        [HttpDelete]
        [Route("{userId}")]
        public async Task<ResponseDto> Delete(string userId)
        {
            if(!Guid.TryParse(userId, out Guid guid))
            {
                return ResponseDto.Fail("Invalid identifier.");
            }

            var succeed = await _identityService.DeleteUserAsync(guid);
            if(!succeed)
            {
                return ResponseDto.Fail("Fail to delete user.");
            }

            return ResponseDto.Success("User has been deleted successfully.");
        }
    }
}