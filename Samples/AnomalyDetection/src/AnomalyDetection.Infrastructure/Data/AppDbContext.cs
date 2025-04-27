using AnomalyDetection.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace AnomalyDetection.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

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