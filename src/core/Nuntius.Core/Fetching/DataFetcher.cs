using Resulz;
using Resulz.Validation;
using Nuntius.Core.Common;

namespace Nuntius.Core.Fetching;

public partial class DataFetcher : Element<string>
{
    #region Properties
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string EngineName { get; protected set; } = string.Empty;
    public virtual string Settings { get; protected set; } = string.Empty;
    #endregion

    #region Ctor
    protected DataFetcher() : base() { }

    protected DataFetcher(string id, string name, string engineName, string settings) : base(id)
        => (Name, EngineName, Settings) = (name, engineName.ToUpper(), settings);
    #endregion

    #region Factory Method
    public static OperationResult<DataFetcher> Create(string id, string name, string engineName, string settings)
        => Validate(id, name, engineName, settings)
            .IfSuccessThenReturn(() => new DataFetcher(id, name, engineName, settings));
    #endregion

    #region Methods
    public virtual OperationResult Update(string name, string engineName, string settings)
        => Validate(Id!, name, engineName, settings)
            .IfSuccess(res => (Name, EngineName, Settings) = (name, engineName.ToUpper(), settings));
    #endregion
}
