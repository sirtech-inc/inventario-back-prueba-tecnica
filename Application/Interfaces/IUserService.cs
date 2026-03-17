using Application.Dtos.User;
using Domain;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<OperationResult<UserDto>> CreateAsync(UserRequestDto saveDto);
    }
}
