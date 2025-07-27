using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Templates.Entities;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.ToTable("Templates");

        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.EngineId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Subject)
            .HasMaxLength(1000);

        builder.Property(t => t.Content)
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.MessageType)
            .HasConversion(
                v => v.Value,
                v => MessageType.FromValue(v))
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion(
                v => v.Value,
                v => ElementStatus.FromValue(v))
            .IsRequired();

        // Configure TemplateContext as an owned entity
        builder.OwnsOne(t => t.Context, ctx =>
        {
            ctx.Property(c => c.Language)
                .HasMaxLength(10)
                .HasColumnName("ContextLanguage")
                .IsRequired();

            ctx.Property(c => c.Scope)
                .HasMaxLength(255)
                .HasColumnName("ContextScope");
        });

        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.HasIndex(t => new { t.EngineId, t.MessageType });
    }
}