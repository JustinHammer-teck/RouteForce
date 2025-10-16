using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class CheckpointsConfiguration : IEntityTypeConfiguration<Checkpoint>
{
    public void Configure(EntityTypeBuilder<Checkpoint> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.AddressLine)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("AddressLine");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("City");

            address.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("State");

            address.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("PostalCode");

            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Country");

            address.Property(a => a.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("Latitude");

            address.Property(a => a.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("Longitude");
        });

        builder.OwnsOne(c => c.ContactPoint, contact =>
        {
            contact.Property(cp => cp.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ContactName");

            contact.Property(cp => cp.Phone)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("ContactPhone");

            contact.Property(cp => cp.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("ContactEmail");
        });

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

        builder.HasOne(c => c.ManagedByBusiness)
            .WithMany(b => b.Warehouses)
            .HasForeignKey(c => c.ManagedByBusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.DeliveryServiceTemplate)
            .WithMany(dst => dst.ServiceCheckpoints)
            .HasForeignKey(c => c.DeliveryServiceTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.RouteCheckpoints)
            .WithOne(rc => rc.Checkpoint)
            .HasForeignKey(rc => rc.CheckpointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CheckpointType);
        builder.HasIndex(c => c.IsActive);
    }
}