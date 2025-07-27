using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Rendering.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class RendererStore : BaseStore<Renderer, string>, IRendererStore
{
    public RendererStore(NuntiusDbContext context) : base(context)
    {
    }
}