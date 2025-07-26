using Resulz;
using Nuntius.Core.Messages;
using Nuntius.Core.Fetching;

namespace Nuntius.Core.Engines;

public interface IDataFetcherEngine
{
    public string Name { get; }
    public OperationResult<string> FetchData(DataFetcher builder, Message message);
}
