using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Messages.Entities;
using Resulz;

namespace Nuntius.Core.Engines;

public interface ISenderEngine
{
    public string Id { get; }
    public Task<OperationResult> SendMessageAsync(Sender messageSender, Message message);
}
