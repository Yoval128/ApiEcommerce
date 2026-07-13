using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;


namespace apiEcommerce.Mapping
{

    public class UserProflile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserLoginResponseDTO>().ReverseMap();
        }
    }


}
