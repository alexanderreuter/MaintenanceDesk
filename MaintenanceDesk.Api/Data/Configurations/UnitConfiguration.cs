using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceDesk.Api.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.Property(u => u.Designation).HasMaxLength(20);

        builder.HasOne<Property>()
            .WithMany()
            .HasForeignKey(u => u.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        // A designation like "1201" is only unique within its own property.
        builder.HasIndex(u => new { u.PropertyId, u.Designation }).IsUnique();

        builder.HasData(SeedData.Units);
    }
}
