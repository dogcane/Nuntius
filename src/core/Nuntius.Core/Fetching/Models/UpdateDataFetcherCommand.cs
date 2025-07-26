using Nuntius.Core.Common;

namespace Nuntius.Core.Fetching.Models;

public record UpdateDataFetcherCommand(
    string Id,
    string Name,
    string EngineName,
    string Settings,
    ElementStatus Status
);