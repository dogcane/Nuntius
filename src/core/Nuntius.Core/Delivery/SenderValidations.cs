using System.Text.Json;
using Nuntius.Core.Messages;
using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Delivery;

public partial class Sender
{
    #region Methods
    protected static OperationResult Validate(string id, string name, string engineName, string settings, MessageType messageType)
        => OperationResult.MakeSuccess()
            .With(id, nameof(id)).Required().StringLength(50).StringMatch("^[a-zA-Z0-9_-]{1,50}$")
            .With(name, nameof(name)).Required().StringLength(200)
            .With(engineName, nameof(engineName)).Required().StringLength(50).StringMatch("^[a-zA-Z0-9_-]{1,50}$")
            .With(settings, nameof(settings)).Required().Condition(payload =>
                {
                    try { JsonDocument.Parse(payload ?? ""); return true; }
                    catch { return false; }
                }, "INVALID_PAYLOAD")
            .With(messageType, nameof(messageType)).Required()
            .Result;
    #endregion
}