using Resulz;
using Nuntius.Core.Common.Entities;

namespace Nuntius.Core.Fetching.Entities;

public partial class DataFetcher : Element<string>
{
    #region Properties
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string EngineId { get; protected set; } = string.Empty;
    public virtual string Settings { get; protected set; } = string.Empty;
    #endregion

    #region Ctor
    protected DataFetcher() : base() { }

    protected DataFetcher(string id, string name, string engineId, string settings) : base(id)
        => (Name, EngineId, Settings) = (name, engineId.ToUpper(), settings);
    #endregion

    #region Factory Method
    public static OperationResult<DataFetcher> Create(string id, string name, string engineId, string settings)
        => Validate(id, name, engineId, settings)
            .IfSuccessThenReturn(() => new DataFetcher(id, name, engineId, settings));
    #endregion

    #region Methods
    public virtual OperationResult Update(string name, string engineId, string settings)
        => ValidateEnable()
        .Then(() => Validate(Id!, name, engineId, settings))
        .IfSuccess(res => (Name, EngineId, Settings) = (name, engineId.ToUpper(), settings));
    #endregion
}
