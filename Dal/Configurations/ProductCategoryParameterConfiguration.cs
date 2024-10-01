using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;

namespace Dal.Configurations;

public class ProductCategoryParameterConfiguration : IEntityTypeConfiguration<ProductCategoryParameter>
{
    public void Configure(EntityTypeBuilder<ProductCategoryParameter> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(100);

        builder.HasMany(p => p.Values)
            .WithOne(p => p.ProductCategoryParameter)
            .HasForeignKey(p => p.ProductCategoryParameterId);

        builder.HasOne(p => p.ProductCategory)
            .WithMany(p => p.Parameters)
            .HasForeignKey(p => p.ProductCategoryId);
    }
}