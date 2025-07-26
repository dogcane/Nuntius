namespace Nuntius.Core.Fetching.Models;

public record CreateDataFetcherCommand(
    string Id,
    string Name,
    string EngineName,
    string Settings
);