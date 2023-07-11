using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recipe.Web.Data;

public class Product
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public AppUser User { get; set; }

    public string Name { get; set; }

    public List<Ingredient> Ingredients { get; set; }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(Product));

        builder.Property(p => p.UserId)
            .IsRequired(true);

        builder.Property(p => p.Name)
            .IsRequired(true)
            .HasMaxLength(256);

        builder.HasIndex(p => new { p.Name, p.UserId });
    }
}
