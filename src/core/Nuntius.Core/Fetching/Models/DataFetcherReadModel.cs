using Nuntius.Core.Common;
using Nuntius.Core.Fetching;

namespace Nuntius.Core.Fetching.Models;

public record DataFetcherReadModel
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string EngineName { get; init; } = string.Empty;
    public string Settings { get; init; } = string.Empty;
    public ElementStatus Status { get; init; } = ElementStatus.Enabled;

    public static DataFetcherReadModel FromEntity(DataFetcher dataFetcher) => new()
    {
        Id = dataFetcher.Id!,
        Name = dataFetcher.Name,
        EngineName = dataFetcher.EngineName,
        Settings = dataFetcher.Settings,
        Status = dataFetcher.Status
    };
}