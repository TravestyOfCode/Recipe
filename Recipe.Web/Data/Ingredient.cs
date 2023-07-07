using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recipe.Web.Data;

public class Ingredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }

    public Recipe Recipe { get; set; }

    public decimal Quantity { get; set; }

    public string Product { get; set; }

    public int? UnitOfMeasureId { get; set; }

    public UnitOfMeasure UnitOfMeasure { get; set; }
}

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable(nameof(Ingredient));

        builder.Property(p => p.Quantity)
            .HasColumnType("decimal(9,5)");

        builder.Property(p => p.Product)
            .IsRequired(true)
            .HasMaxLength(64);
    }
}
