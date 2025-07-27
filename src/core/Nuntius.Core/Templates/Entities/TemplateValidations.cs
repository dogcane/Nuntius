using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages.Entities;
using Resulz;
using Resulz.Validation;

namespace Nuntius.Core.Templates.Entities;

public partial class Template
{
    #region Methods
    protected static OperationResult Validate(string id, string name, string engineId, TemplateContext context, string subject, string content, MessageType messageType)
        => OperationResult.MakeSuccess()
            .With(id, nameof(id)).ValidId()
            .With(name, nameof(name)).Required().StringLength(100)
            .With(engineId, nameof(engineId)).ValidId()
            .With(context, nameof(context)).Required()
            .With(context?.Language, $"{nameof(context)}.{nameof(context.Language)}").Required().StringLength(2)
            .With(context?.Scope, $"{nameof(context)}.{nameof(context.Scope)}").StringLength(50)
            .With(subject, nameof(subject)).Required().StringLength(100)
            .With(content, nameof(content)).Required()
            .With(messageType, nameof(messageType)).Required()
            .Result;
    #endregion
}
