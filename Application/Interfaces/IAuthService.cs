using Application.Dtos.Auth;
using Domain;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<OperationResult<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    }
}
