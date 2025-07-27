using Nuntius.Core.Rendering.Entities;
using Nuntius.Core.Rendering.Infrastructure;

namespace Nuntios.Core.Storage.EF.Stores;

public class RendererStore(NuntiusDbContext context) : EfStore<Renderer, string>(context), IRendererStore
{
}