using AnomalyDetection.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnomalyDetection.Data.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<NetworkTrafficLog> NetworkTrafficLogs { get; set; }
    public DbSet<AnomalyAlert> AnomalyAlerts { get; set; }
    public DbSet<MLModel> MLModels { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure NetworkTrafficLog entity
        builder.Entity<NetworkTrafficLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SourceIp).IsRequired().HasMaxLength(45);
            entity.Property(e => e.DestinationIp).IsRequired().HasMaxLength(45);
            entity.Property(e => e.Protocol).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Timestamp).IsRequired();
        });

        // Configure AnomalyAlert entity
        builder.Entity<AnomalyAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AlertType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.SeverityLevel).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Timestamp).IsRequired();

            // Configure many-to-many relationship with NetworkTrafficLogs
            entity.HasMany(e => e.RelatedLogs)
                  .WithMany()
                  .UsingEntity(j => j.ToTable("AlertRelatedLogs"));
        });

        // Configure MLModel entity
        builder.Entity<MLModel>(entity =>
        {
            // Primary key configuration
            entity.HasKey(e => e.Id);

            // Name property
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Description property
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            // ModelType property (required)
            entity.Property(e => e.ModelType)
                .IsRequired()
                .HasMaxLength(50);

            // FilePath property (required)
            entity.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(255);

            // CreatedAt property (required)
            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // LastTrainedAt property (required)
            entity.Property(e => e.LastTrainedAt)
                .IsRequired();

            // LastEvaluatedAt property (optional)
            entity.Property(e => e.LastEvaluatedAt)
                .IsRequired(false);

            // TrainingParameters property (optional)
            entity.Property(e => e.TrainingParameters)
                .HasMaxLength(1000);

            // NumberOfFeatures property (optional)
            entity.Property(e => e.NumberOfFeatures)
                .IsRequired(false);

            // TrainingDataSize property (optional)
            entity.Property(e => e.TrainingDataSize)
                .IsRequired(false);

            // Evaluation metrics properties (required, as they should be populated after training/evaluation)
            entity.Property(e => e.Accuracy)
                .IsRequired();

            entity.Property(e => e.Precision)
                .IsRequired();

            entity.Property(e => e.Recall)
                .IsRequired();

            entity.Property(e => e.F1Score)
                .IsRequired();

            // DecisionThreshold property (optional)
            entity.Property(e => e.DecisionThreshold)
                .IsRequired(false);

            // Version property (default value "1.0")
            entity.Property(e => e.Version)
                .HasDefaultValue("1.0")
                .HasMaxLength(10);

            // IsProduction property (default value false)
            entity.Property(e => e.IsProduction)
                .HasDefaultValue(false);

            // Metadata property: this is a dictionary, we'll map it to a JSON column if using PostgreSQL, or handle it differently depending on the database
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v))
                .HasMaxLength(2000) // Assuming a maximum length for serialized JSON
                .IsRequired(false);
        });
    }
}
