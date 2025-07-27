using Nuntius.Core.Common.Entities;

namespace Nuntius.Core.Common.Infrastructure;

public interface IStore<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}