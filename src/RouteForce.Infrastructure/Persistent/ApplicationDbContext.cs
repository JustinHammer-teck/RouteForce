using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RouteForce.Application.Common.Interfaces;
using RouteForce.Core.Models;

namespace RouteForce.Infrastructure.Persistent;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<WebhookToken> WebhookTokens { get; }
    public DbSet<Notification> Notifications { get; }
    public DbSet<RouteCheckpoint> RouteCheckpoints { get; }
    public DbSet<Checkpoint> Checkpoints { get; }
    public DbSet<User> Users { get; }
    public DbSet<Business> Businesses { get; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}