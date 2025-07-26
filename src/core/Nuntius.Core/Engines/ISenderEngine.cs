using Nuntius.Core.Delivery;
using Nuntius.Core.Messages;
using Resulz;

namespace Nuntius.Core.Engines;

public interface ISenderEngine
{
    public string Name { get; }
    public Task<OperationResult> SendMessageAsync(Sender messageSender, Message message);
}
