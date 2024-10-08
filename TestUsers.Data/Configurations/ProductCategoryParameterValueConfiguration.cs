using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class ProductCategoryParameterValueConfiguration : IEntityTypeConfiguration<ProductCategoryParameterValue>
{
    public void Configure(EntityTypeBuilder<ProductCategoryParameterValue> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Value)
            .HasMaxLength(500);

        builder.Property(p => p.DateCreated);

        builder.HasMany(p => p.ProductCategoryParameterValueProduct)
            .WithOne(p => p.ProductCategoryParameterValue)
            .HasForeignKey(p => p.ProductCategoryParameterValueId);

        builder.HasOne(p => p.ProductCategoryParameter)
            .WithMany(p => p.Values)
            .HasForeignKey(p => p.ProductCategoryParameterId);

        builder.HasMany(p => p.UserSaveFilter)
            .WithOne(p => p.ProductCategoryParameterValue)
            .HasForeignKey(p => p.ProductCategoryParameterValueId);
    }
}