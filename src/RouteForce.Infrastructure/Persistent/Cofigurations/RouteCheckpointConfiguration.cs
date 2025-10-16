using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Enums;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class RouteCheckpointConfiguration : IEntityTypeConfiguration<RouteCheckpoint>
{
    public void Configure(EntityTypeBuilder<RouteCheckpoint> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.SequenceNumber)
            .IsRequired();

        builder.Property(rc => rc.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(RouteCheckPointStatus.Pending);

        builder.Property(rc => rc.Notes)
            .HasMaxLength(1000);

        builder.HasOne(rc => rc.Order)
            .WithMany(o => o.RouteCheckpoints)
            .HasForeignKey(rc => rc.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.Checkpoint)
            .WithMany(c => c.RouteCheckpoints)
            .HasForeignKey(rc => rc.CheckpointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(rc => rc.ConfirmedByToken)
            .WithMany()
            .HasForeignKey(rc => rc.ConfirmedByTokenId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(rc => rc.OrderId);
        builder.HasIndex(rc => rc.Status);
        builder.HasIndex(rc => new { rc.OrderId, rc.SequenceNumber });
    }
}