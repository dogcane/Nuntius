using Nuntius.Core.Common.Entities;

namespace Nuntius.Core.Messages;

public class MessageAttachment : Entity<int>
{
    #region Properties
    public virtual Message Message { get; protected set; } = null!;
    public virtual string Name { get; protected set; } = string.Empty;
    public virtual string ContentType { get; protected set; } = string.Empty;
    #endregion

    #region Ctor
    protected MessageAttachment() : base() { }
    public MessageAttachment(Message message, string name, string contentType) : this()
        => (Message, Name, ContentType) = (message, name, contentType);
    #endregion
}
