using Nuntius.Core.Messages;
using Nuntius.Core.Templates.Entities;
using Resulz;

namespace Nuntius.Core.Engines;

public interface IRenderEngine
{
    public string Name { get; }
    public OperationResult<RenderedMessage> RenderMessage(Template template, Message message);
}
