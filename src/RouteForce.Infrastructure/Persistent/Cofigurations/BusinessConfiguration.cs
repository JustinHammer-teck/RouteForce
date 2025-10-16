using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsOne(b => b.BusinessAddress, address =>
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

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.HasMany(b => b.Warehouses)
            .WithOne(c => c.ManagedByBusiness)
            .HasForeignKey(c => c.ManagedByBusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Orders)
            .WithOne(o => o.Business)
            .HasForeignKey(o => o.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Users)
            .WithOne()
            .HasForeignKey(u => u.BusinessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.IsActive);
    }
}
