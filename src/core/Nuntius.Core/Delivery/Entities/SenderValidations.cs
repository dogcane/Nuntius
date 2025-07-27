using System.Text.Json;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages;
using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Delivery.Entities;

public partial class Sender
{
    #region Methods
    protected static OperationResult Validate(string id, string name, string engineId, string settings, MessageType messageType)
        => OperationResult.MakeSuccess()
            .With(id, nameof(id)).ValidId()
            .With(name, nameof(name)).Required().StringLength(100)
            .With(engineId, nameof(engineId)).ValidId()
            .With(settings, nameof(settings)).Required().Condition(payload =>
                {
                    try { JsonDocument.Parse(payload ?? ""); return true; }
                    catch { return false; }
                }, "INVALID_PAYLOAD")
            .With(messageType, nameof(messageType)).Required()
            .Result;
    #endregion
}