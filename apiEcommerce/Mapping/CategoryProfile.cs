using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using AutoMapper;

namespace apiEcommerce.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            //RevereMap para que también pueda mapear en ambas direcciones

            CreateMap<Category, CreateCategoryDto>().ReverseMap();
        }
    }
}
