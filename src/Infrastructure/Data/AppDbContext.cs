using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<FaultReport> FaultReports { get; set; }
    public DbSet<FaultStatusLog> FaultStatusLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SQL Server: User -> FaultReport -> FaultStatusLog ve User -> FaultStatusLog (ChangedById)
        // ikisi de CASCADE olunca "multiple cascade paths" hatası verir.
        modelBuilder.Entity<FaultStatusLog>()
            .HasOne(l => l.ChangedBy)
            .WithMany()
            .HasForeignKey(l => l.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}