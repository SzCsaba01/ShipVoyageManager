using Microsoft.EntityFrameworkCore;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Access.Data;
public class ShipVoyageManagerContext : DbContext
{
    public ShipVoyageManagerContext(DbContextOptions<ShipVoyageManagerContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PortEntity>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<UserEntity>()
            .HasIndex(p => p.Username)
            .IsUnique();

        modelBuilder.Entity<UserEntity>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<ShipEntity>()
            .HasIndex(s => s.Name)
            .IsUnique();

        modelBuilder.Entity<VoyageEntity>()
            .HasOne(v => v.DeparturePort)
            .WithMany(p => p.DepartingVoyages)
            .HasForeignKey(v => v.DeparturePortId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VoyageEntity>()
            .HasOne(v => v.ArrivalPort)
            .WithMany(p => p.ArrivingVoyages)
            .HasForeignKey(v => v.ArrivalPortId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<VoyageEntity> Voyages { get; set; }
    public DbSet<ShipEntity> Ships { get; set; }
    public DbSet<PortEntity> Ports { get; set; }
    public DbSet<VisitedCountryEntity> VisitedCountries { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
}
