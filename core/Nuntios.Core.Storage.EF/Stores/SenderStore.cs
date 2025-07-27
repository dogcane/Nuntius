using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Delivery.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class SenderStore(NuntiusDbContext context) : EfStore<Sender, string>(context), ISenderStore
{
}