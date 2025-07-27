using System.Text.Json;
using Nuntius.Core.Common.Entities;
using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Rendering.Entities;

public partial class Renderer
{
    #region Methods

    protected static OperationResult Validate(string id, string name, string engineId, string settings)
        => OperationResult.MakeSuccess()
            .With(id, nameof(id)).ValidId()
            .With(name, nameof(name)).Required().StringLength(200)
            .With(engineId, nameof(engineId)).ValidId()
            .With(settings, nameof(settings)).Required().Condition(payload =>
                {
                    try { JsonDocument.Parse(payload ?? ""); return true; }
                    catch { return false; }
                }, "INVALID_PAYLOAD")
            .Result;

    
    #endregion
}