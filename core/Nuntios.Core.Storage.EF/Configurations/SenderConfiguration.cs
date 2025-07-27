using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class SenderConfiguration : IEntityTypeConfiguration<Sender>
{
    public void Configure(EntityTypeBuilder<Sender> builder)
    {
        builder.ToTable("Senders");

        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(s => s.EngineId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.Settings)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.MessageType)
            .HasConversion(
                v => v.Value,
                v => MessageType.FromValue(v))
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion(
                v => v.Value,
                v => ElementStatus.FromValue(v))
            .IsRequired();

        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.HasIndex(s => new { s.EngineId, s.MessageType });
    }
}