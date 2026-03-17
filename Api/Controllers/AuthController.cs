using Application.Dtos.Auth;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<Results<BadRequest<OperationResult<LoginResponseDto>>, Ok<OperationResult<LoginResponseDto>>>> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);

            if (response.Success) return TypedResults.Ok(response);

            return TypedResults.BadRequest(response);
        }
    }
}
