using Nuntius.Core.Engines;

namespace Nuntius.Core.Engines;

public interface IEngineFactory
{
    public IDataFetcherEngine GetDataFetcherEngine(string engineName);
    public bool TryGetDataFetcherEngine(string engineName, out IDataFetcherEngine? engine);
    public IRenderEngine GetRenderEngine(string engineName);
    public bool TryGetRenderEngine(string engineName, out IRenderEngine? engine);
    public ISenderEngine GetSenderEngine(string engineName);
    public bool TryGetSenderEngine(string engineName, out ISenderEngine? engine);
}
