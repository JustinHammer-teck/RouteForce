using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class RouteCheckpointConfiguration : IEntityTypeConfiguration<RouteCheckpoint>
{
    public void Configure(EntityTypeBuilder<RouteCheckpoint> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.OrderID)
            .IsRequired();

        builder.Property(rc => rc.CheckpointID)
            .IsRequired();

        builder.Property(rc => rc.ManagedByBusinessID)
            .IsRequired();

        builder.Property(rc => rc.SequenceNumber)
            .IsRequired();

        builder.Property(rc => rc.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(rc => rc.ConfirmedByTokenID)
            .IsRequired(false);

        builder.Property(rc => rc.ConfirmationTimestamp)
            .IsRequired(false);

        builder.Property(rc => rc.ExpectedArrival)
            .IsRequired(false);

        builder.Property(rc => rc.ActualArrival)
            .IsRequired(false);

        builder.HasIndex(rc => rc.OrderID);
        builder.HasIndex(rc => rc.CheckpointID);
        builder.HasIndex(rc => rc.Status);
        builder.HasIndex(rc => new { rc.OrderID, rc.SequenceNumber });
    }
}