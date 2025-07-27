using Resulz;
using Nuntius.Core.Fetching.Entities;
using Nuntius.Core.Messages.Entities;

namespace Nuntius.Core.Engines;

public interface IDataFetcherEngine
{
    public string Id { get; }
    public OperationResult<string> FetchData(DataFetcher builder, Message message);
}
