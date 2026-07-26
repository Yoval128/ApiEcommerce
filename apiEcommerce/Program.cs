using apiEcommerce.Constants;
using apiEcommerce.Repository;
using apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Get the connection string from the configuration and register the ApplicationDbContext
// with the dependency injection container
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Register the category repository with the dependency injection container

builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register the product repository with the dependency injection container

builder.Services.AddScoped<IUserRepository, UserRepository>(); // Register the user repository with the dependency injection container

builder.Services.AddAutoMapper(cfg => // Register AutoMapper and add the mapping profiles from the assembly
{
    cfg.AddMaps(typeof(Program).Assembly);
});


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
        ValidateAudience = true, // Enable audience validation to ensure the token is intended for this application
    };
});

//Controllers are added to the service collection, which allows the application to
//handle HTTP requests and return responses
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CROSS configuration

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

app.UseAuthorization();

app.MapControllers();

app.Run();
