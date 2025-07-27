namespace Nuntius.Core.Engines;

public class EngineFactory : IEngineFactory
{
    #region Fields
    protected readonly Dictionary<string, IDataFetcherEngine> fetcherEngines = [];
    protected readonly Dictionary<string, IRenderEngine> renderEngines = [];
    protected readonly Dictionary<string, ISenderEngine> senderEngines = [];
    #endregion

    #region Ctor
    public EngineFactory(IEnumerable<IDataFetcherEngine> builderEngines, IEnumerable<IRenderEngine> renderEngines, IEnumerable<ISenderEngine> senderEngines)
    {
        ArgumentNullException.ThrowIfNull(builderEngines, nameof(builderEngines));
        ArgumentNullException.ThrowIfNull(renderEngines, nameof(renderEngines));
        ArgumentNullException.ThrowIfNull(senderEngines, nameof(senderEngines));
        foreach (var modelEngine in builderEngines)
        {
            this.fetcherEngines.TryAdd(modelEngine.Id, modelEngine);
        }
        foreach (var viewEngine in renderEngines)
        {
            this.renderEngines.TryAdd(viewEngine.Id, viewEngine);
        }
        foreach (var senderEngine in senderEngines)
        {
            this.senderEngines.TryAdd(senderEngine.Id, senderEngine);
        }
    }    
    #endregion

    #region Methods
    public IDataFetcherEngine GetDataFetcherEngine(string engineId) 
        => fetcherEngines.TryGetValue(engineId, out var engine) ? engine : throw new ArgumentException($"Engine with id {engineId} not found", nameof(engineId));

    public bool TryGetDataFetcherEngine(string engineId, out IDataFetcherEngine? engine)
        => fetcherEngines.TryGetValue(engineId, out engine);

    public IRenderEngine GetRenderEngine(string engineId) 
        => renderEngines.TryGetValue(engineId, out var engine) ? engine : throw new ArgumentException($"Engine with id {engineId} not found", nameof(engineId));

    public bool TryGetRenderEngine(string engineId, out IRenderEngine? engine)
        => renderEngines.TryGetValue(engineId, out engine);

    public ISenderEngine GetSenderEngine(string engineId)
        => senderEngines.TryGetValue(engineId, out var engine) ? engine : throw new ArgumentException($"Engine with id {engineId} not found", nameof(engineId));

    public bool TryGetSenderEngine(string engineId, out ISenderEngine? engine)
        => senderEngines.TryGetValue(engineId, out engine);
    #endregion
}
