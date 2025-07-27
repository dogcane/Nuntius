using Resulz;
using Nuntius.Core.Messages;
using Nuntius.Core.Fetching.Entities;

namespace Nuntius.Core.Engines;

public interface IDataFetcherEngine
{
    public string Id { get; }
    public OperationResult<string> FetchData(DataFetcher builder, Message message);
}
