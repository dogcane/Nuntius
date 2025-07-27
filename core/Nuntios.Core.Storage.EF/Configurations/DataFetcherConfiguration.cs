using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nuntius.Core.Fetching.Entities;

namespace Nuntios.Core.Storage.EF.Configurations;

public class DataFetcherConfiguration : ElementConfiguration<DataFetcher>
{
    public override void Configure(EntityTypeBuilder<DataFetcher> builder)
    {
        base.Configure(builder);

        builder.ToTable("DataFetchers");

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.EngineId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Settings)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        // Indexes for common queries
        builder.HasIndex(d => d.Name);
        builder.HasIndex(d => d.EngineId);
    }
}