using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaintenanceDesk.Api.Data.Configurations;

public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        builder.Property(r => r.Title).HasMaxLength(200);
        builder.Property(r => r.Description).HasMaxLength(2000);
        builder.Property(r => r.ResolutionNotes).HasMaxLength(2000);

        // Restrict rather than cascade: request history must outlive what it refers to
        // + SQL Server rejects the two cascade paths from Unit (direct, and via Resident).
        builder.HasOne<Unit>()
            .WithMany()
            .HasForeignKey(r => r.UnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Resident>()
            .WithMany()
            .HasForeignKey(r => r.ReportedByResidentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Technician>()
            .WithMany()
            .HasForeignKey(r => r.AssignedTechnicianId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
