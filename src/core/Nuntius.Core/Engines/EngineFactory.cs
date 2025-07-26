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
            this.fetcherEngines.TryAdd(modelEngine.Name, modelEngine);
        }
        foreach (var viewEngine in renderEngines)
        {
            this.renderEngines.TryAdd(viewEngine.Name, viewEngine);
        }
        foreach (var senderEngine in senderEngines)
        {
            this.senderEngines.TryAdd(senderEngine.Name, senderEngine);
        }
    }    
    #endregion

    #region Methods
    public IDataFetcherEngine GetDataFetcherEngine(string engineName) 
        => fetcherEngines.TryGetValue(engineName, out var engine) ? engine : throw new ArgumentException($"Engine with name {engineName} not found", nameof(engineName));

    public bool TryGetDataFetcherEngine(string engineName, out IDataFetcherEngine? engine)
        => fetcherEngines.TryGetValue(engineName, out engine);

    public IRenderEngine GetRenderEngine(string engineName) 
        => renderEngines.TryGetValue(engineName, out var engine) ? engine : throw new ArgumentException($"Engine with name {engineName} not found", nameof(engineName));

    public bool TryGetRenderEngine(string engineName, out IRenderEngine? engine)
        => renderEngines.TryGetValue(engineName, out engine);

    public ISenderEngine GetSenderEngine(string engineName)
        => senderEngines.TryGetValue(engineName, out var engine) ? engine : throw new ArgumentException($"Engine with name {engineName} not found", nameof(engineName));

    public bool TryGetSenderEngine(string engineName, out ISenderEngine? engine)
        => senderEngines.TryGetValue(engineName, out engine);
    #endregion
}
