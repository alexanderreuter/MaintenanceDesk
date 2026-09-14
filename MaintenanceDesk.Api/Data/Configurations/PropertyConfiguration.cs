using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceDesk.Api.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(200);
        builder.Property(p => p.StreetAddress).HasMaxLength(200);
        builder.Property(p => p.PostalCode).HasMaxLength(20);
        builder.Property(p => p.City).HasMaxLength(100);
    }
}
