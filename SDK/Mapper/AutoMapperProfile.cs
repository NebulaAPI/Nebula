using AutoMapper;
using Nebula.SDK.Objects.DTO;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}