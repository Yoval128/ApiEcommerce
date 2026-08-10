using apiEcommerce.Constants;
using apiEcommerce.Models;
using apiEcommerce.Repository;
using apiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connection string from the configuration and register the ApplicationDbContext
// with the dependency injection container
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      // Seeding de Roles
      if (!appContext.Roles.Any())
      {
          appContext.Roles.AddRange(
            new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
          );
      }
      // Seeding de Categorías
      if (!appContext.Categories.Any())
      {
          appContext.Categories.AddRange(
            new Category { Name = "Ropa y accesorios", CreationDate = DateTime.Now },
            new Category { Name = "Electrónicos", CreationDate = DateTime.Now },
            new Category { Name = "Deportes", CreationDate = DateTime.Now },
            new Category { Name = "Hogar", CreationDate = DateTime.Now },
            new Category { Name = "Libros", CreationDate = DateTime.Now }
          );
      }
      // Seeding de Usuario Administrador
      if (!appContext.ApplicationUsers.Any())
      {
          var hasher = new PasswordHasher<ApplicationUser>();
          var adminUser = new ApplicationUser
          {
              Id = "admin-001",
              UserName = "admin@admin.com",
              NormalizedUserName = "ADMIN@ADMIN.COM",
              Email = "admin@admin.com",
              NormalizedEmail = "ADMIN@ADMIN.COM",
              EmailConfirmed = true,
              Name = "Administrador"
          };
          adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!");

          var regularUser = new ApplicationUser
          {
              Id = "user-001",
              UserName = "user@user.com",
              NormalizedUserName = "USER@USER.COM",
              Email = "user@user.com",
              NormalizedEmail = "USER@USER.COM",
              EmailConfirmed = true,
              Name = "Usuario Regular"
          };
          regularUser.PasswordHash = hasher.HashPassword(regularUser, "User123!");

          appContext.ApplicationUsers.AddRange(adminUser, regularUser);
      }
      // Seeding de UserRoles
      if (!appContext.UserRoles.Any())
      {
          appContext.UserRoles.AddRange(
            new IdentityUserRole<string> { UserId = "admin-001", RoleId = "1" }, // Admin
            new IdentityUserRole<string> { UserId = "user-001", RoleId = "2" }   // User
          );
      }

      // Seeding de Productos
      if (!appContext.Products.Any())
      {
          appContext.Products.AddRange(
            new Product
            {
                Name = "Camiseta Básica",
                Description = "Camiseta de algodón 100%",
                Price = 25.99m,
                SKU = "PROD-001-CAM-M",
                Stock = 50,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/FF0000/FFFFFF?text=Camiseta",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Smartphone Galaxy",
                Description = "Teléfono inteligente con 128GB",
                Price = 599.99m,
                SKU = "PROD-002-PHO-BLK",
                Stock = 25,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/0000FF/FFFFFF?text=Smartphone",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Pelota de Fútbol",
                Description = "Pelota oficial FIFA",
                Price = 45.00m,
                SKU = "PROD-003-BAL-WHT",
                Stock = 30,
                CategoryId = 3,
                Category = appContext.Categories.Find(3)!,
                ImgUrl = "https://via.placeholder.com/300x300/00FF00/FFFFFF?text=Pelota",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Lámpara de Mesa",
                Description = "Lámpara LED regulable",
                Price = 89.99m,
                SKU = "PROD-004-LAM-WHT",
                Stock = 15,
                CategoryId = 4,
                Category = appContext.Categories.Find(4)!,
                ImgUrl = "https://via.placeholder.com/300x300/FFFF00/000000?text=Lampara",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "El Quijote",
                Description = "Novela clásica de Cervantes",
                Price = 19.99m,
                SKU = "PROD-005-LIB-ESP",
                Stock = 100,
                CategoryId = 5,
                Category = appContext.Categories.Find(5)!,
                ImgUrl = "https://via.placeholder.com/300x300/800080/FFFFFF?text=Libro",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Jeans Clásicos",
                Description = "Pantalones vaqueros azules",
                Price = 79.99m,
                SKU = "PROD-006-PAN-BLU",
                Stock = 40,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/4169E1/FFFFFF?text=Jeans",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Tablet Pro",
                Description = "Tablet 10.5 pulgadas con stylus incluido",
                Price = 459.99m,
                SKU = "PROD-007-TAB-SIL",
                Stock = 20,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/C0C0C0/000000?text=Tablet",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Zapatillas Running",
                Description = "Zapatillas deportivas para correr",
                Price = 129.99m,
                SKU = "PROD-008-ZAP-BLK",
                Stock = 35,
                CategoryId = 3,
                Category = appContext.Categories.Find(3)!,
                ImgUrl = "https://via.placeholder.com/300x300/000000/FFFFFF?text=Zapatillas",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Cafetera Express",
                Description = "Cafetera automática con molinillo integrado",
                Price = 299.99m,
                SKU = "PROD-009-CAF-BLK",
                Stock = 12,
                CategoryId = 4,
                Category = appContext.Categories.Find(4)!,
                ImgUrl = "https://via.placeholder.com/300x300/2F4F4F/FFFFFF?text=Cafetera",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Programación en C#",
                Description = "Guía completa de programación en C# y .NET",
                Price = 49.99m,
                SKU = "PROD-010-LIB-ESP",
                Stock = 80,
                CategoryId = 5,
                Category = appContext.Categories.Find(5)!,
                ImgUrl = "https://via.placeholder.com/300x300/008B8B/FFFFFF?text=C%23+Book",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Chaqueta Deportiva",
                Description = "Chaqueta impermeable para actividades al aire libre",
                Price = 149.99m,
                SKU = "PROD-011-CHA-NAV",
                Stock = 28,
                CategoryId = 1,
                Category = appContext.Categories.Find(1)!,
                ImgUrl = "https://via.placeholder.com/300x300/000080/FFFFFF?text=Chaqueta",
                CreationDate = DateTime.Now
            },
            new Product
            {
                Name = "Auriculares Bluetooth",
                Description = "Auriculares inalámbricos con cancelación de ruido",
                Price = 189.99m,
                SKU = "PROD-012-AUR-BLK",
                Stock = 45,
                CategoryId = 2,
                Category = appContext.Categories.Find(2)!,
                ImgUrl = "https://via.placeholder.com/300x300/1C1C1C/FFFFFF?text=Auriculares",
                CreationDate = DateTime.Now
            }
          );
      }
      appContext.SaveChanges();
  })
);

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024; // Define the one megabite 
    options.UseCaseSensitivePaths = true; // Configurate sensibility for case and 
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Register the category repository with the dependency injection container

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register the product repository with the dependency injection container

builder.Services.AddScoped<IUserRepository, UserRepository>(); // Register the user repository with the dependency injection container

builder.Services.AddAutoMapper(cfg => // Register AutoMapper and add the mapping profiles from the assembly
{
    cfg.AddMaps(typeof(Program).Assembly);
});

// Configure Indetity services for user authentication and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()  // Use the ApplicationDbContext for storing user and role information
.AddDefaultTokenProviders(); // Add default token providers for generating tokens for password reset, email confirmation, etc.

// Get the secret key from the configuration for JWT token validation
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Secret key is not configured in appsettings.json");
}

// Add authentication services to the service collection and configure the default authentication scheme to use JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme to JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default challenge scheme to JWT Bearer
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Disable HTTPS metadata requirement for development purposes
    options.SaveToken = true; // Save the token in the authentication properties

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validate the signing key of the token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // Use the secret key to validate the token's signature
        ValidateIssuer = false, // Disable issuer validation for simplicity
        ValidateAudience = false,
    };
});

//Controllers are added to the service collection, which allows the application to
//handle HTTP requests and return responses
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);

    options.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
  {
      // Add a security definition for JWT Bearer authentication to the Swagger documentation
      options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
      {
          Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                      "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                      "Ejemplo: \"12345abcdef\"",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.Http,
          Scheme = "Bearer"
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
      {
        new OpenApiSecurityScheme
        {
          Reference = new OpenApiReference
          {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer" //Use the name of the schema created 
          },
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });

      options.SwaggerDoc("v1", new OpenApiInfo
      {
          Version = "v1",
          Title = "ApiEcomers",
          Description = "API for managing products and users",
          TermsOfService = new Uri("http://example.com/Terms"),
          Contact = new OpenApiContact
          {
              Name = "YovalDev",
              Url = new Uri("https://yoval-dev.vercel.app/"),

          },
          License = new OpenApiLicense
          {
              Name = "Use License",
              Url = new Uri("http://example.com/license"),
          }
      });

      options.SwaggerDoc("v2", new OpenApiInfo
      {
          Version = "v2",
          Title = "ApiEcommerce",
          Description = "APIs for manageming user and products",
          TermsOfService = new Uri("http://example.com/Terms"),
          Contact = new OpenApiContact
          {
              Name = "YovalDev",
              Url = new Uri("https://yoval-dev.vercel.app/"),
          },
          License = new OpenApiLicense
          {
              Name = "Use License",
              Url = new Uri("http://example.com/Terms"),
          }
      });
  });

// Configure API versioning.
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    // If the client does not specify an API version, use the default version.
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Set the default API version.
    options.DefaultApiVersion = new ApiVersion(1, 0);

    // Include the supported and deprecated API versions in the response headers.
    options.ReportApiVersions = true;

    // Read the API version from the query string (?api-version=1.0).
    //  options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version"));
});

apiVersioningBuilder.AddApiExplorer(options =>
{
    // Define the API version group name format (v1, v2, v3, etc.).
    options.GroupNameFormat = "'v'VVV";

    // Replace the API version placeholder in the URL with the actual version.
    options.SubstituteApiVersionInUrl = true;
});

//Cors configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
        policy =>
        {
            policy.AllowAnyOrigin()   // Allow requests from any origin, and you can add more with a comma-separated list of origins example: "http://example.com,http://anotherexample.com"
                  .AllowAnyMethod()   // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                  .AllowAnyHeader();  // Allow any header
        });
});

var app = builder.Build();

Console.WriteLine($"ContentRoot: {app.Environment.ContentRootPath}");
Console.WriteLine($"WebRoot: {app.Environment.WebRootPath}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.UseStaticFiles(); // Enable serving static files from the wwwroot folder

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
