using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Messages.Entities;
using System.Text.Json;

namespace Nuntios.Core.Storage.EF.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        // Primary key
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();

        builder.Property(m => m.BuilderCode)
            .HasMaxLength(200);

        builder.Property(m => m.TemplateCode)
            .HasMaxLength(200);

        builder.Property(m => m.SenderCode)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.From)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Payload)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(m => m.MessageType)
            .HasConversion(
                messageType => messageType.Value,
                value => MessageType.FromValue(value))
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion(
                status => status.Value,
                value => MessageStatus.FromValue(value))
            .IsRequired();

        builder.Property(m => m.CreatedOn)
            .IsRequired();

        builder.Property(m => m.CompiledOn);

        builder.Property(m => m.GeneratedOn);

        builder.Property(m => m.SentOn);

        builder.Property(m => m.Retries)
            .IsRequired();

        // Complex type mapping for MessageRecipients
        builder.Property(m => m.Recipients)
            .HasConversion(
                recipients => JsonSerializer.Serialize(recipients, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<MessageRecipients>(json, (JsonSerializerOptions?)null) ?? new MessageRecipients())
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        // Complex type mapping for RenderedMessage (nullable)
        builder.Property(m => m.RenderedMessage)
            .HasConversion(
                rendered => rendered == null ? null : JsonSerializer.Serialize(rendered, (JsonSerializerOptions?)null),
                json => string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<RenderedMessage>(json, (JsonSerializerOptions?)null))
            .HasColumnType("nvarchar(max)");

        // Indexes for common queries
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.MessageType);
        builder.HasIndex(m => m.CreatedOn);
        builder.HasIndex(m => m.SenderCode);
        builder.HasIndex(m => m.TemplateCode);
        builder.HasIndex(m => m.BuilderCode);

        // Ignore computed properties
        builder.Ignore(m => m.PayloadHeader);
        builder.Ignore(m => m.PayloadBody);
    }
}