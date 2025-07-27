namespace Nuntius.Core.Messages.Entities;

public record MessageRecipients(string To, string[] Cc, string[] Bcc)
{
    public MessageRecipients(): this(string.Empty, [], []) { }
}
