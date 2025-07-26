using Nuntius.Core.Common;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Templates.Entities;

public partial class Template : Element<string>
{
    #region Properties
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string EngineName { get; protected set; } = string.Empty;
    public virtual TemplateContext Context { get; protected set; } = new();
    public virtual string Subject { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual MessageType MessageType { get; protected set; } = MessageType.Text;
    #endregion

    #region Ctor
    protected Template() : base() { }
    
    protected Template(string id, string name, string engineName, TemplateContext context, string subject, string content, MessageType messageType) 
        : base(id)
        => (Name, EngineName, Context, Subject, Content, MessageType) = 
           (name.ToUpper(), engineName.ToUpper(), context, subject, content, messageType);
    #endregion

    #region Factory Method
    public static OperationResult<Template> Create(string id, string name, string engineName, TemplateContext context, string subject, string content, MessageType messageType)
        => Validate(id, name, engineName, context, subject, content, messageType)
            .IfSuccessThenReturn(() => new Template(id, name, engineName, context, subject, content, messageType));
    #endregion

    #region Methods
    public virtual OperationResult Update(string engineName, string subject, string content)
        => ValidateStatus(this.Status, this.Status)
            .IfSuccess(_ => Validate(this.Id!, this.Name, engineName, this.Context, subject, content, this.MessageType))
            .IfSuccess(res => (EngineName, Subject, Content) = (engineName.ToUpper(), subject, content));
    #endregion
}
