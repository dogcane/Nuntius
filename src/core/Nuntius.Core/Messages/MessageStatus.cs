namespace Nuntius.Core.Messages;

public sealed record MessageStatus
{
    private MessageStatus(int value, string name) 
        => (Value, Name) = (value, name);

    public int Value { get; }
    public string Name { get; }

    public static MessageStatus New => new(0, nameof(New));
    public static MessageStatus Built => new(1, nameof(Built));
    public static MessageStatus Rendered => new(2, nameof(Rendered));
    public static MessageStatus Sent => new(3, nameof(Sent));
    public static MessageStatus Fault => new(4, nameof(Fault));

    public static MessageStatus[] All =>
    [
        New,
        Built,
        Rendered,
        Sent,
        Fault
    ];

    public static MessageStatus FromValue(int value)
        => All.FirstOrDefault(s => s.Value == value) 
           ?? throw new ArgumentException($"Invalid message status value: {value}", nameof(value));

    public override string ToString() => Name;

    public bool IsEndStatus => this == Sent || this == Fault;
}
