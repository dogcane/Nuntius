using System.Linq.Expressions;
using Nuntius.Core.Fetching;

namespace Nuntius.Core.Fetching.Models;

public static class DataFetcherProjections
{
    public static Expression<Func<DataFetcher, DataFetcherReadModel>> ToReadModel =>
        dataFetcher => new DataFetcherReadModel
        {
            Id = dataFetcher.Id!,
            Name = dataFetcher.Name,
            EngineName = dataFetcher.EngineName,
            Settings = dataFetcher.Settings,
            Status = dataFetcher.Status
        };

    public static IQueryable<DataFetcherReadModel> ProjectToReadModel(this IQueryable<DataFetcher> query)
        => query.Select(ToReadModel);
}