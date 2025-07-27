namespace Nuntius.Core.Messages.Entities;

public record RenderedMessage(string Subject, string Content)
{
    public RenderedMessage() : this(string.Empty, string.Empty) { }
}
