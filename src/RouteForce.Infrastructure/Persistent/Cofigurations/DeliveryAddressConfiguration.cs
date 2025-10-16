using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class DeliveryAddressConfiguration : IEntityTypeConfiguration<DeliveryAddress>
{
    public void Configure(EntityTypeBuilder<DeliveryAddress> builder)
    {
        builder.HasKey(da => da.Id);

        builder.Property(da => da.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(da => da.Address, address =>
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

        builder.Property(da => da.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(da => da.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(da => da.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(da => da.Notes)
            .HasMaxLength(1000);

        builder.HasOne(da => da.PersonalReceiver)
            .WithMany(pr => pr.SavedAddresses)
            .HasForeignKey(da => da.PersonalReceiverId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(da => da.PersonalReceiverId);
        builder.HasIndex(da => new { da.PersonalReceiverId, da.IsDefault });
        builder.HasIndex(da => da.IsActive);
    }
}
