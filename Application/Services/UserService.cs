using Application.Dtos.User;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Entities;
using Infraestructure.Persistences.Interfaces;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<OperationResult<UserDto>> CreateAsync(UserRequestDto saveDto)
        {
            var userExists = await _userRepository.ExistsByPropertyAsync("Username", saveDto.Username);

            if (userExists)
            {
                return OperationResult<UserDto>.Fail("Ya existe un usuario con el mismo username.");
            }

            User user = _mapper.Map<User>(saveDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(saveDto.Password);

            await _userRepository.SaveAsync(user);

            return new OperationResult<UserDto>()
            {
                Success = true,
                Data = _mapper.Map<UserDto>(user),
                Message = "Usuario creado con éxito"
            };
        }
    }
}
