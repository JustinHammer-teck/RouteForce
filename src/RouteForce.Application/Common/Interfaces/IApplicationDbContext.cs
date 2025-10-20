using Microsoft.EntityFrameworkCore;
using RouteForce.Core.Models;

namespace RouteForce.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WebhookToken> WebhookTokens { get; }
    DbSet<RouteCheckpoint> RouteCheckpoints { get; }
    DbSet<Checkpoint> Checkpoints { get; }
    DbSet<User> Users { get; }
    DbSet<Business> Businesses { get; }
    DbSet<PersonalReceiver> PersonalReceivers { get; }
    DbSet<DeliveryAddress> DeliveryAddresses { get; }
    DbSet<Order> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}