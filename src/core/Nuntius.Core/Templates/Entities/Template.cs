using Nuntius.Core.Common.Entities;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Templates.Entities;

public partial class Template : Element<string>
{
    #region Properties
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string EngineId { get; protected set; } = string.Empty;
    public virtual TemplateContext Context { get; protected set; } = new();
    public virtual string Subject { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual MessageType MessageType { get; protected set; } = MessageType.Text;
    #endregion

    #region Ctor
    protected Template() : base() { }
    
    protected Template(string id, string name, string engineId, TemplateContext context, string subject, string content, MessageType messageType) 
        : base(id)
        => (Name, EngineId, Context, Subject, Content, MessageType) = 
           (name.ToUpper(), engineId.ToUpper(), context, subject, content, messageType);
    #endregion

    #region Factory Method
    public static OperationResult<Template> Create(string id, string name, string engineId, TemplateContext context, string subject, string content, MessageType messageType)
        => Validate(id, name, engineId, context, subject, content, messageType)
            .IfSuccessThenReturn(() => new Template(id, name, engineId, context, subject, content, messageType));
    #endregion

    #region Methods
    public virtual OperationResult Update(string engineId, string subject, string content)
        => ValidateEnable()
        .Then(() => Validate(this.Id!, this.Name, engineId, this.Context, subject, content, this.MessageType))
        .IfSuccess(res => (EngineId, Subject, Content) = (engineId.ToUpper(), subject, content));
    #endregion
}
