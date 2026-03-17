using Application.Dtos.User;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappers
{
    public class UserMappingsProfile : Profile
    {
        public UserMappingsProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserRequestDto, User>();
        }
    }
}
