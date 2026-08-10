using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;


namespace apiEcommerce.Mapping
{

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserLoginResponseDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDataDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }


}
