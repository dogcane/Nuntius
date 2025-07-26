using Nuntius.Core.Messages;
using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Templates.Entities;

public partial class Template
{
    #region Methods
    protected static OperationResult Validate(string id, string name, string engineName, TemplateContext context, string subject, string content, MessageType messageType)
        => OperationResult.MakeSuccess()
            .With(id, nameof(id)).Required().StringLength(50).StringMatch("^[a-zA-Z0-9_-]{1,50}$")
            .With(name, nameof(name)).Required().StringLength(50).StringMatch("^[a-zA-Z0-9_-]{1,50}$")
            .With(engineName, nameof(engineName)).Required().StringLength(50).StringMatch("^[a-zA-Z0-9_-]{1,50}$")
            .With(context, nameof(context)).Required()
            .With(context?.Language, $"{nameof(context)}.{nameof(context.Language)}").Required().StringLength(2)
            .With(context?.Scope, $"{nameof(context)}.{nameof(context.Scope)}").StringLength(50)
            .With(subject, nameof(subject)).Required().StringLength(100)
            .With(content, nameof(content)).Required()
            .With(messageType, nameof(messageType)).Required()
            .Result;
    #endregion
}
