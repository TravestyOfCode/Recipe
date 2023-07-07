using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Recipe.Web.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Category> Categories { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Recipe> Recipes { get; set; }

    public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }
}

public class AppDbContextDesignFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(config.GetConnectionString("Default"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
