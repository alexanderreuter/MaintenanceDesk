using MaintenanceDesk.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceDesk.Api.Data;

public class MaintenanceDeskDbContext(DbContextOptions<MaintenanceDeskDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();

    public DbSet<Unit> Units => Set<Unit>();

    public DbSet<Resident> Residents => Set<Resident>();

    public DbSet<Technician> Technicians => Set<Technician>();

    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaintenanceDeskDbContext).Assembly);
    }
}
