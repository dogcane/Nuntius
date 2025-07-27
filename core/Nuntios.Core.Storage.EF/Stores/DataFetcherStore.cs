using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Fetching.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class DataFetcherStore : EfStore<DataFetcher, string>, IDataFetcherStore
{
    public DataFetcherStore(NuntiusDbContext context) : base(context)
    {
    }
}