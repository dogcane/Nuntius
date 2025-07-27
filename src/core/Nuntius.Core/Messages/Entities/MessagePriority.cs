namespace Nuntius.Core.Messages.Entities;

public sealed record MessagePriority
{
    private MessagePriority(int value, string name) 
        => (Value, Name) = (value, name);

    public int Value { get; }
    public string Name { get; }

    public static MessagePriority Critical => new(1, nameof(Critical));
    public static MessagePriority High => new(10, nameof(High));
    public static MessagePriority Normal => new(50, nameof(Normal));
    public static MessagePriority Low => new(99, nameof(Low));

    public static MessagePriority[] All =>
    [
        Critical,
        High,
        Normal,
        Low
    ];

    public static MessagePriority FromValue(int value)
        => All.FirstOrDefault(p => p.Value == value) 
           ?? throw new ArgumentException($"Invalid priority value: {value}", nameof(value));

    public static MessagePriority Default => Normal;
    public override string ToString() => Name;
}