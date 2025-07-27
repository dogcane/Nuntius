using Microsoft.EntityFrameworkCore;
using Nuntius.Core.Common.Entities;
using Nuntius.Core.Common.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public abstract class EfStore<TEntity, TKey> : IStore<TEntity, TKey>
    where TEntity : Entity<TKey>
    where TKey : notnull
{
    protected readonly NuntiusDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected EfStore(NuntiusDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // Satisfy async requirement
        return _dbSet.AsQueryable();
    }

    public virtual async Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var existingEntity = await GetByIdAsync(entity.Id, cancellationToken);
        
        if (existingEntity == null)
        {
            _dbSet.Add(entity);
        }
        else
        {
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var existingEntity = await GetByIdAsync(entity.Id, cancellationToken);
        if (existingEntity != null)
        {
            _dbSet.Remove(existingEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}