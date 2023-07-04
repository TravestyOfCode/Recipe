using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recipe.Web.Data;

public class Category
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public AppUser User { get; set; }

    public string Name { get; set; }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(nameof(Category));

        builder.Property(p => p.Name)
            .IsRequired(true)
            .HasMaxLength(32);

        builder.HasIndex(p => new { p.Name, p.UserId });
    }
}
