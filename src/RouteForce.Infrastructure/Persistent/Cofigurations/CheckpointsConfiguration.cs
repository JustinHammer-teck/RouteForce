using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class CheckpointsConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CheckpointName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.AddressLine)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Latitude)
            .HasPrecision(9, 6);

        builder.Property(c => c.Longitude)
            .HasPrecision(9, 6);

        builder.Property(c => c.ContactName)
            .HasMaxLength(200);

        builder.Property(c => c.ContactPhone)
            .HasMaxLength(50);

        builder.Property(c => c.ContactEmail)
            .HasMaxLength(255);

        builder.Property(c => c.CheckpointType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.RequiresConfirmation)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(c => c.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(c => c.CheckpointType);
        builder.HasIndex(c => c.IsActive);
        builder.HasIndex(c => new { c.City, c.State });
    }
}