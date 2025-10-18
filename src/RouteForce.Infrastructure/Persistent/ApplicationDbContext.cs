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

    public DbSet<WebhookToken> WebhookTokens { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<RouteCheckpoint> RouteCheckpoints { get; set; }
    public DbSet<Checkpoint> Checkpoints { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Business> Businesses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}