using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Recipe.Web.Data;

public class UnitOfMeasure
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public AppUser User { get; set; }

    public string Name { get; set; }

    public string Abbreviation { get; set; }

    public decimal? ConversionToGramsRatio { get; set; }
}

public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable(nameof(UnitOfMeasure));

        builder.Property(p => p.Name)
            .IsRequired(true)
            .HasMaxLength(16);

        builder.Property(p => p.Abbreviation)
            .IsRequired(true)
            .HasMaxLength(8);

        builder.Property(p => p.ConversionToGramsRatio)
            .HasColumnType("decimal(9,5)");

        builder.HasIndex(p => new { p.Name, p.UserId })
            .IsUnique(true);

        builder.HasIndex(p => new { p.Abbreviation, p.UserId })
            .IsUnique(true);
    }
}
