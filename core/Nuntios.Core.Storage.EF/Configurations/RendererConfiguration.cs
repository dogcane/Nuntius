using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Rendering.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class RendererConfiguration : ElementConfiguration<Renderer>
{
    public override void Configure(EntityTypeBuilder<Renderer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Renderers");

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.EngineId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Settings)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Indexes for common queries
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.EngineId);
    }
}