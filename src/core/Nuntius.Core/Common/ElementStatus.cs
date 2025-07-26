namespace Nuntius.Core.Common;

public sealed record ElementStatus
{
    private ElementStatus(int value, string name)
        => (Value, Name) = (value, name);

    public int Value { get; }
    public string Name { get; }

    public static ElementStatus Enabled => new(0, nameof(Enabled));
    public static ElementStatus Disabled => new(1, nameof(Disabled));
    public static ElementStatus Archived => new(2, nameof(Archived));

    public static ElementStatus[] All =>
    [
        Enabled,
        Disabled,
        Archived
    ];

    public static ElementStatus FromValue(int value) 
        => All.FirstOrDefault(s => s.Value == value) 
           ?? throw new ArgumentException($"Invalid status value: {value}", nameof(value));

    public override string ToString() => Name;
}
