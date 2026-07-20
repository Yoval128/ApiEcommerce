using apiEcommerce.Repository;
using apiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;


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


//Controllers are added to the service collection, which allows the application to
//handle HTTP requests and return responses
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
