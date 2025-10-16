using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class DeliveryServiceTemplateConfiguration : IEntityTypeConfiguration<DeliveryServiceTemplate>
{
    public void Configure(EntityTypeBuilder<DeliveryServiceTemplate> builder)
    {
        builder.HasKey(dst => dst.Id);

        builder.Property(dst => dst.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(dst => dst.Description)
            .HasMaxLength(1000);

        builder.Property(dst => dst.ServiceCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(dst => dst.EstimatedDeliveryDays)
            .IsRequired();

        builder.Property(dst => dst.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(dst => dst.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(dst => dst.ServiceCheckpoints)
            .WithOne(c => c.DeliveryServiceTemplate)
            .HasForeignKey(c => c.DeliveryServiceTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(dst => dst.Orders)
            .WithOne(o => o.DeliveryServiceTemplate)
            .HasForeignKey(o => o.DeliveryServiceTemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(dst => dst.ServiceCode)
            .IsUnique();

        builder.HasIndex(dst => dst.IsActive);
    }
}
