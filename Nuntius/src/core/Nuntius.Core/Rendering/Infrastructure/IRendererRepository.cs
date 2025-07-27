// filepath: Nuntius/src/core/Nuntius.Core/Rendering/Infrastructure/IRendererRepository.cs
using Nuntius.Core.Common.Entities;

namespace Nuntius.Core.Rendering.Infrastructure
{
    public interface IRendererRepository
    {
        void Add(Renderer renderer);
        void Update(Renderer renderer);
        Renderer GetById(string id);
    }
}