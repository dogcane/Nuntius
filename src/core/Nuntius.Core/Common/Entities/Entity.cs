namespace Nuntius.Core.Common.Entities;

public abstract class Entity<TKey> : IEquatable<Entity<TKey>>
    where TKey : notnull
{
    public virtual TKey Id { get; protected set; } = default!;

    protected Entity() { }

    protected Entity(TKey id)
        => Id = id ?? throw new ArgumentNullException(nameof(id), "Id cannot be null.");

    public override bool Equals(object? obj)
        => Equals(obj as Entity<TKey>);

    public override int GetHashCode()
        => HashCode.Combine(GetType(), Id);

    public bool Equals(Entity<TKey>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(Id, other.Id);
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
        => !(left == right);
}