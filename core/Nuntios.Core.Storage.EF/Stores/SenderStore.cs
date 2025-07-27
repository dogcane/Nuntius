using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Delivery.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class SenderStore : BaseStore<Sender, string>, ISenderStore
{
    public SenderStore(NuntiusDbContext context) : base(context)
    {
    }
}