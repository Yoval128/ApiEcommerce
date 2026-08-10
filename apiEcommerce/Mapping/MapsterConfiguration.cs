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

            // ApplicationUser mappings
            // Mapster does not automatically map UserName → Username because the property names are different.
            // These explicit mappings ensure that Identity's UserName is correctly mapped to the DTO's Username.
            // TwoWays() also enables the reverse mapping: Username → UserName.

            config.NewConfig<ApplicationUser, UserDataDTO>()
                .Map(dest => dest.Username, src => src.UserName)
                .TwoWays();

            config.NewConfig<ApplicationUser, UserDTO>()
                .Map(dest => dest.Username, src => src.UserName)
                .TwoWays();

        }
    }
}
