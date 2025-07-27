using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Common.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class RendererConfiguration : IEntityTypeConfiguration<Renderer>
{
    public void Configure(EntityTypeBuilder<Renderer> builder)
    {
        builder.ToTable("Renderers");

        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.EngineId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.Settings)
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.Status)
            .HasConversion(
                v => v.Value,
                v => ElementStatus.FromValue(v))
            .IsRequired();

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasIndex(r => r.EngineId);
    }
}