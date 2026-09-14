using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceDesk.Api.Data.Configurations;

public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
{
    public void Configure(EntityTypeBuilder<Resident> builder)
    {
        builder.Property(r => r.FullName).HasMaxLength(200);
        // 254 is the longest address SMTP will deliver to.
        builder.Property(r => r.Email).HasMaxLength(254);
        builder.Property(r => r.PhoneNumber).HasMaxLength(50);

        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(r => r.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
