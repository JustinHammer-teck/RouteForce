using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TrackingNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(o => o.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.OwnsOne(o => o.DeliveryAddress, address =>
        {
            address.Property(a => a.AddressLine)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("DeliveryAddressLine");

            address.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("DeliveryCity");

            address.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("DeliveryState");

            address.Property(a => a.PostalCode)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("DeliveryPostalCode");

            address.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("DeliveryCountry");

            address.Property(a => a.Latitude)
                .HasPrecision(9, 6)
                .HasColumnName("DeliveryLatitude");

            address.Property(a => a.Longitude)
                .HasPrecision(9, 6)
                .HasColumnName("DeliveryLongitude");
        });

        builder.HasOne(o => o.Business)
            .WithMany(b => b.Orders)
            .HasForeignKey(o => o.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.PersonalReceiver)
            .WithMany(pr => pr.Orders)
            .HasForeignKey(o => o.PersonalReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DeliveryServiceTemplate)
            .WithMany(dst => dst.Orders)
            .HasForeignKey(o => o.DeliveryServiceTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.SelectedDeliveryAddress)
            .WithMany()
            .HasForeignKey(o => o.SelectedDeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.DeliveryCheckpoint)
            .WithMany()
            .HasForeignKey(o => o.DeliveryCheckpointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.RouteCheckpoints)
            .WithOne(rc => rc.Order)
            .HasForeignKey(rc => rc.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Notifications)
            .WithOne(n => n.Order)
            .HasForeignKey(n => n.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.WebhookTokens)
            .WithOne(wt => wt.Order)
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.TrackingNumber)
            .IsUnique();

        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedDate);
        builder.HasIndex(o => new { o.BusinessId, o.Status });
    }
}
