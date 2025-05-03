using AnomalyDetection.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AnomalyDetection.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<NetworkTrafficLog> NetworkTrafficLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the NetworkTrafficLog entity
        modelBuilder.Entity<NetworkTrafficLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SourceIp).IsRequired();
            entity.Property(e => e.DestinationIp).IsRequired();
            entity.Property(e => e.Protocol).IsRequired();
            entity.Property(e => e.UserAgent).IsRequired(false);
        });
    }
}
