using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Messages.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class SenderConfiguration : ElementConfiguration<Sender>
{
    public override void Configure(EntityTypeBuilder<Sender> builder)
    {
        base.Configure(builder);

        builder.ToTable("Senders");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.EngineId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Settings)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.MessageType)
            .HasConversion(
                messageType => messageType.Value,
                value => MessageType.FromValue(value))
            .IsRequired();

        // Indexes for common queries
        builder.HasIndex(s => s.Name);
        builder.HasIndex(s => s.EngineId);
        builder.HasIndex(s => s.MessageType);
    }
}