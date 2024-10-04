using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestUsers.Data.Models;

namespace TestUsers.Data.Configurations;

public class UserSaveFilterRelationConfiguration : IEntityTypeConfiguration<UserSaveFilterRelation>
{
    public void Configure(EntityTypeBuilder<UserSaveFilterRelation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(p => p.UserSaveFilter)
            .WithMany(p => p.CategoryParametersValues)
            .HasForeignKey(p => p.UserSaveFilterId);

        builder.HasOne(p => p.ProductCategoryParameterValue)
            .WithMany(p => p.UserSaveFilter)
            .HasForeignKey(p => p.ProductCategoryParameterValueId);
    }
}
