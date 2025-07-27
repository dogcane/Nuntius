using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Templates.Entities;
using Nuntius.Core.Messages.Entities;
using System.Text.Json;

namespace Nuntios.Core.Storage.EF.Configurations;

public class TemplateConfiguration : ElementConfiguration<Template>
{
    public override void Configure(EntityTypeBuilder<Template> builder)
    {
        base.Configure(builder);

        builder.ToTable("Templates");

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.EngineId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Subject)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.MessageType)
            .HasConversion(
                messageType => messageType.Value,
                value => MessageType.FromValue(value))
            .IsRequired();

        // Complex type mapping for TemplateContext
        builder.Property(t => t.Context)
            .HasConversion(
                context => JsonSerializer.Serialize(context, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<TemplateContext>(json, (JsonSerializerOptions?)null) ?? new TemplateContext())
            .HasColumnType("nvarchar(max)");

        // Indexes for common queries
        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.EngineId);
        builder.HasIndex(t => t.MessageType);
    }
}