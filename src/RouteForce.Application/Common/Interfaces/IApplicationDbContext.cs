using Microsoft.EntityFrameworkCore;
using RouteForce.Core.Models;

namespace RouteForce.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<WebhookToken> WebhookTokens { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<RouteCheckpoint> RouteCheckpoints { get; }
    DbSet<Checkpoint> Checkpoints { get; }
    DbSet<User> Users { get; }
}