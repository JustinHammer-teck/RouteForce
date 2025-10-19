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

        builder.Property(wt => wt.IssueType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasColumnName("IssueType");

        builder.Property(wt => wt.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(wt => wt.Order)
            .WithMany(o => o.WebhookTokens)
            .HasForeignKey(wt => wt.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(wt => wt.OrderId);
        builder.HasIndex(wt => wt.IsActive);
        builder.HasIndex(wt => wt.ExpirationDate);
        builder.HasIndex(wt => wt.IssueType);
    }
}