using Microsoft.EntityFrameworkCore;
using Nuntius.Core.Messages.Entities;
using Nuntius.Core.Messages.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class MessageStore : IMessageStore
{
    private readonly NuntiusDbContext _context;
    private readonly DbSet<Message> _dbSet;

    public MessageStore(NuntiusDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<Message>();
    }

    public virtual async Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IQueryable<Message>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_dbSet.AsQueryable());
    }

    public virtual async Task<Message> SaveAsync(Message entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Check if entity exists (Id = 0 means new entity for auto-increment)
        var existingEntity = entity.Id > 0 ? await GetByIdAsync(entity.Id, cancellationToken) : null;
        
        if (existingEntity == null)
        {
            // Add new entity
            await _dbSet.AddAsync(entity, cancellationToken);
        }
        else
        {
            // Update existing entity
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(Message entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}