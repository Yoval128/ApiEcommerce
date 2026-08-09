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
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

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

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
