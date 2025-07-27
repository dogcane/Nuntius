namespace Nuntius.Core.Messages.Entities;

public sealed record MessageType
{
    private MessageType(int value, string name) 
        => (Value, Name) = (value, name);

    public int Value { get; }
    public string Name { get; }

    public static MessageType Text => new(0, nameof(Text));
    public static MessageType Email => new(1, nameof(Email));
    public static MessageType Notification => new(2, nameof(Notification));

    public static MessageType[] All =>
    [
        Text,
        Email,
        Notification
    ];

    public static MessageType FromValue(int value)
        => All.FirstOrDefault(t => t.Value == value) 
           ?? throw new ArgumentException($"Invalid message type value: {value}", nameof(value));

    public override string ToString() => Name;
}
