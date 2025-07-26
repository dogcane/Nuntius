namespace Nuntius.Core.Messages;

public record MessageRecipients(string To, string[] Cc, string[] Bcc)
{
    public MessageRecipients(): this(string.Empty, [], []) { }
}
