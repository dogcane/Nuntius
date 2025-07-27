using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Fetching.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class DataFetcherStore : BaseStore<DataFetcher, string>, IDataFetcherStore
{
    public DataFetcherStore(NuntiusDbContext context) : base(context)
    {
    }
}