using apiEcommerce.Models;
using Microsoft.EntityFrameworkCore;

// The ApplicationDbContext class inherits from DbContext,
// which is a part of the Entity Framework Core library.
// This class is responsible for managing the database
// connection and providing access to the database tables through DbSet properties.
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }


    // The OnModelCreating method is overridden to configure the model
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    // The Categories property is a DbSet of Category
    // entities, which represents the Categories
    // table in the database.
    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
}

