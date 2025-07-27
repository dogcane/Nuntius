namespace Nuntius.Core.Engines;

public interface IEngineFactory
{
    public IDataFetcherEngine GetDataFetcherEngine(string engineId);
    public bool TryGetDataFetcherEngine(string engineId, out IDataFetcherEngine? engine);
    public IRenderEngine GetRenderEngine(string engineId);
    public bool TryGetRenderEngine(string engineId, out IRenderEngine? engine);
    public ISenderEngine GetSenderEngine(string engineId);
    public bool TryGetSenderEngine(string engineId, out ISenderEngine? engine);
}
