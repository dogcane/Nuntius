using Nuntius.Core.Common;

namespace Nuntius.Core.Fetching;

public interface IDataFetcherStore : IStore<DataFetcher, string>
{
    Task<IQueryable<DataFetcher>> GetByEngineNameAsync(string engineName, CancellationToken cancellationToken = default);
    Task<IQueryable<DataFetcher>> GetActiveAsync(CancellationToken cancellationToken = default);
}