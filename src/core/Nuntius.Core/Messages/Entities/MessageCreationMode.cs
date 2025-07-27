namespace Nuntius.Core.Messages.Entities;

public sealed record MessageCreationMode
{
    private MessageCreationMode(int value, string name) 
        => (Value, Name) = (value, name);

    public int Value { get; }
    public string Name { get; }

    public static MessageCreationMode FromBuilder => new(0, nameof(FromBuilder));
    public static MessageCreationMode FromTemplate => new(1, nameof(FromTemplate));
    public static MessageCreationMode FromRenderedMessage => new(2, nameof(FromRenderedMessage));

    public static MessageCreationMode[] All =>
    [
        FromBuilder,
        FromTemplate,
        FromRenderedMessage
    ];

    public static MessageCreationMode FromValue(int value)
        => All.FirstOrDefault(m => m.Value == value) 
           ?? throw new ArgumentException($"Invalid creation mode value: {value}", nameof(value));

    public override string ToString() => Name;
}

