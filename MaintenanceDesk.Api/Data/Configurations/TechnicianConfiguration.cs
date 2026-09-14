using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceDesk.Api.Data.Configurations;

public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.Property(t => t.FullName).HasMaxLength(200);
        builder.Property(t => t.Email).HasMaxLength(254);
        builder.Property(t => t.Trade).HasMaxLength(100);
    }
}
