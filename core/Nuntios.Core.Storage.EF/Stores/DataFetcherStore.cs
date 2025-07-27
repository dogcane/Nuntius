using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Fetching.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class DataFetcherStore(NuntiusDbContext context) : EfStore<DataFetcher, string>(context), IDataFetcherStore
{
}