using Nuntius.Core.Messages.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class MessageStore(NuntiusDbContext context) : IMessageStore
{
    private readonly NuntiusDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    // Currently the IMessageStore interface is empty, so no methods to implement
    // This can be extended when the interface adds methods
}