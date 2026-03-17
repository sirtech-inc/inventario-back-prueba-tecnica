using Application.Dtos.User;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Results<BadRequest<OperationResult<UserDto>>, Ok<OperationResult<UserDto>>>> Post([FromBody] UserRequestDto request)
        {
            var response = await _userService.CreateAsync(request);

            if (response.Success) return TypedResults.Ok(response);

            return TypedResults.BadRequest(response);
        }
    }
}
