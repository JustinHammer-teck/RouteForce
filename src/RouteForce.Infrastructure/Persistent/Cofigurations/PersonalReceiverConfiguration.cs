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
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pr => pr.Notes)
            .HasMaxLength(1000);

        builder.Property(pr => pr.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(pr => pr.CreatedByBusiness)
            .WithMany()
            .HasForeignKey(pr => pr.CreatedByBusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pr => pr.Orders)
            .WithOne(o => o.PersonalReceiver)
            .HasForeignKey(o => o.PersonalReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pr => pr.SavedAddresses)
            .WithOne(da => da.PersonalReceiver)
            .HasForeignKey(da => da.PersonalReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.Email);
        builder.HasIndex(pr => pr.CreatedByBusinessId);
    }
}