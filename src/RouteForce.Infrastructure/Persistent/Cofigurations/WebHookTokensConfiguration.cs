using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent.Cofigurations;

public class WebHookTokensConfiguration : IEntityTypeConfiguration<WebhookToken>
{
    public void Configure(EntityTypeBuilder<WebhookToken> builder)
    {
        builder.HasKey(wt => wt.Id);

        builder.OwnsOne(wt => wt.Token, token =>
        {
            token.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("TokenValue");

            token.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasColumnName("TokenType");

            token.Property(t => t.IssuedToType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasColumnName("IssuedToType");
        });

        builder.Property(wt => wt.OrderID)
            .IsRequired();

        builder.Property(wt => wt.IssuedToBusinessID)
            .IsRequired(false);

        builder.Property(wt => wt.IssuedToPersonalReceiverID)
            .IsRequired(false);

        builder.Property(wt => wt.ExpirationDate)
            .IsRequired();

        builder.Property(wt => wt.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(wt => wt.UsageLimit)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(wt => wt.UsedCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(wt => wt.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(wt => wt.OrderID);
        builder.HasIndex(wt => wt.IsActive);
        builder.HasIndex(wt => wt.ExpirationDate);
    }
}