using Nuntius.Core.Delivery.Entities;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Engines;

public interface ISenderEngine
{
    public string Id { get; }
    public Task<OperationResult> SendMessageAsync(Sender messageSender, Message message);
}
