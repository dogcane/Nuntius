using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Common.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class DataFetcherConfiguration : IEntityTypeConfiguration<DataFetcher>
{
    public void Configure(EntityTypeBuilder<DataFetcher> builder)
    {
        builder.ToTable("DataFetchers");

        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Id)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.Name)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.EngineId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.Settings)
            .HasColumnType("nvarchar(max)");

        builder.Property(d => d.Status)
            .HasConversion(
                v => v.Value,
                v => ElementStatus.FromValue(v))
            .IsRequired();

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.HasIndex(d => d.EngineId);
    }
}