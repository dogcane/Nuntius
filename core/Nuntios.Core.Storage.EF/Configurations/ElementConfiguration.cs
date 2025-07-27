using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Common.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public abstract class ElementConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Element<string>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .IsRequired()
            .HasMaxLength(200);

        // Status configuration
        builder.Property(e => e.Status)
            .HasConversion(
                status => status.Value,
                value => ElementStatus.FromValue(value))
            .IsRequired();

        // Index on Status for efficient queries
        builder.HasIndex(e => e.Status);
    }
}