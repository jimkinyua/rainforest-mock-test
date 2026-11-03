using Certify.ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Certify.ApiService.Data;

public class CertifyDbContext(DbContextOptions<CertifyDbContext> options) : DbContext(options)
{
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<Audit> Audits => Set<Audit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.Property(f => f.Name).HasMaxLength(200);
            entity.Property(f => f.Country).HasMaxLength(100);
            entity.HasMany(f => f.Audits)
                  .WithOne(a => a.Farm)
                  .HasForeignKey(a => a.FarmId);
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.Property(a => a.Auditor).HasMaxLength(200);
        });
    }
}
