using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class PersonalReceiverConfiguration : IEntityTypeConfiguration<PersonalReceiver>
{
    public void Configure(EntityTypeBuilder<PersonalReceiver> builder)
    {
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pr => pr.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(pr => pr.Phone)
            .HasMaxLength(50);

        builder.Property(pr => pr.Notes)
            .HasMaxLength(1000);

        builder.Property(pr => pr.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(pr => pr.CheckpointID)
            .IsRequired();

        builder.Property(pr => pr.CreatedByBusinessID)
            .IsRequired();

        builder.HasIndex(pr => pr.Email);
        builder.HasIndex(pr => pr.CheckpointID);
        builder.HasIndex(pr => pr.CreatedByBusinessID);
    }
}