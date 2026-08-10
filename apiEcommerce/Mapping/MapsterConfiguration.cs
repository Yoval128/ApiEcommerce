using apiEcommerce.Models;
using apiEcommerce.Models.Dtos;
using Mapster;

namespace apiEcommerce.Mapping
{
    public static class MapsterConfiguration
    {
        public static void Configure(TypeAdapterConfig config)
        {
            // Product mappings
            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
                .TwoWays();

            config.NewConfig<Product, CreateProductDto>().TwoWays();
            config.NewConfig<Product, UpdateProductDto>().TwoWays();

            // Category mappings
            config.NewConfig<Category, CategoryDto>().TwoWays();
            config.NewConfig<Category, CreateCategoryDto>().TwoWays();

            // User mappings
            config.NewConfig<User, UserDTO>().TwoWays();
            config.NewConfig<User, CreateUserDTO>().TwoWays();
            config.NewConfig<User, UserLoginDTO>().TwoWays();
            config.NewConfig<User, UserLoginResponseDTO>().TwoWays();
            config.NewConfig<ApplicationUser, UserDataDTO>().TwoWays();
            config.NewConfig<ApplicationUser, UserDTO>().TwoWays();
        }
    }
}
