using Microsoft.EntityFrameworkCore;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Messages.Entities;
using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Templates.Entities;

namespace Nuntios.Core.Storage.EF;

public class NuntiusDbContext : DbContext
{
    public NuntiusDbContext(DbContextOptions<NuntiusDbContext> options) : base(options)
    {
    }

    public DbSet<Sender> Senders { get; set; } = null!;
    public DbSet<Renderer> Renderers { get; set; } = null!;
    public DbSet<Template> Templates { get; set; } = null!;
    public DbSet<DataFetcher> DataFetchers { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NuntiusDbContext).Assembly);
    }
}