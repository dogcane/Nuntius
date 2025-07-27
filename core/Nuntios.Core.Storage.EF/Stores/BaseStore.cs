using Microsoft.EntityFrameworkCore;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Common.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public abstract class BaseStore<TEntity, TKey> : IStore<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    protected readonly NuntiusDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseStore(NuntiusDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(DbSet.AsQueryable());
    }

    public virtual async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Check if entity exists
        var existingEntity = await GetByIdAsync(entity.Id, cancellationToken);
        
        if (existingEntity == null)
        {
            // Add new entity
            await DbSet.AddAsync(entity, cancellationToken);
        }
        else
        {
            // Update existing entity
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}