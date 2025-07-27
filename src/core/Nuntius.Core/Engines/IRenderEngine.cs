using Nuntius.Core.Messages.Entities;
using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Templates.Entities;
using Resulz;

namespace Nuntius.Core.Engines;

public interface IRenderEngine
{
    public string Id { get; }
    public OperationResult<RenderedMessage> RenderMessage(Renderer renderer, Template template, Message message);
}
